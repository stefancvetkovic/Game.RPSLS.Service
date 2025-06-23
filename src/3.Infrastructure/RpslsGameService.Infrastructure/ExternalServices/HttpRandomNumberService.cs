using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Domain;
using RpslsGameService.Infrastructure.Configuration;
using RpslsGameService.Infrastructure.Models;
using System.Net;
using System.Text.Json;

namespace RpslsGameService.Infrastructure.ExternalServices;

/// <summary>
/// Enhanced HTTP Random Number Service with comprehensive resilience patterns.
/// Implements Circuit Breaker, Retry, and Timeout policies using Polly for robust external service integration.
/// 
/// Resilience Features:
/// 1. Circuit Breaker - Prevents cascading failures by monitoring failure rates
/// 2. Retry Policy - Automatically retries failed requests with exponential backoff
/// 3. Timeout Policy - Prevents hanging requests from blocking the application
/// 4. Fallback Strategy - Gracefully degrades to local random generation
/// 5. Comprehensive Logging - Detailed observability for monitoring and debugging
/// </summary>
public sealed class HttpRandomNumberService : IRandomNumberService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpRandomNumberService> _logger;
    private readonly RandomNumberServiceSettings _settings;
    
    // Polly resilience pipeline combining multiple policies
    private readonly ResiliencePipeline<int> _resiliencePipeline;
    
    // Static JSON serializer options for better performance
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    
    // Pre-allocated strings for logging to reduce allocations
    private const string LogDebugRandomRequest = "Requesting random number from external API with resilience policies";
    private const string LogDebugRandomSuccess = "Successfully received random number from external API: {RandomNumber}";
    private const string LogDebugApiRequest = "Making HTTP request to external random number service";
    private const string LogDebugApiSuccess = "Successfully parsed random number from external API: {RandomNumber}";
    private const string LogInfoFallback = "Using fallback random number due to external service failure: {FallbackNumber}";
    private const string LogWarningInvalidNumber = "Invalid random number received from API: {RandomNumber}";
    private const string LogWarningJsonParse = "Failed to parse JSON response: '{Content}'";
    private const string LogWarningApiStatus = "External API returned non-success status: {StatusCode} {ReasonPhrase}";
    private const string LogWarningCircuitBreaker = "Circuit breaker is open - external random number service is unavailable";
    private const string LogErrorTimeout = "Request to random number service timed out after all retry attempts";
    private const string LogErrorGeneral = "Failed to get random number from external service after all resilience policies";

    public HttpRandomNumberService(
        HttpClient httpClient,
        IOptions<ExternalApiSettings> externalApiSettings,
        ILogger<HttpRandomNumberService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = externalApiSettings?.Value?.RandomNumberService ?? throw new ArgumentNullException(nameof(externalApiSettings));
        
        // Configure the HttpClient timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        
        // Build comprehensive resilience pipeline
        _resiliencePipeline = BuildResiliencePipeline();
    }

    /// <summary>
    /// Retrieves a random number from an external API using comprehensive resilience patterns.
    /// This method leverages a Polly resilience pipeline that includes Circuit Breaker, Retry, and Timeout policies
    /// to provide robust error handling and automatic failover to local random generation.
    /// 
    /// Resilience Pipeline Order:
    /// 1. Timeout Policy - Prevents hanging requests
    /// 2. Circuit Breaker - Monitors failure rate and prevents cascading failures
    /// 3. Retry Policy - Automatically retries failed requests with exponential backoff
    /// 4. Core HTTP Request - The actual external API call
    /// 5. Fallback Strategy - Local random generation if all else fails
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for request timeout and cancellation support.
    /// Allows the operation to be cancelled if it takes too long or if the application is shutting down.
    /// </param>
    /// <returns>
    /// A random integer, either from the external service or from the fallback generator.
    /// The range depends on the external service, but fallback generates numbers 1-100.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when both the resilience pipeline fails AND fallback is disabled in configuration.
    /// This represents a terminal failure condition that requires immediate attention.
    /// </exception>
    public async Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug(LogDebugRandomRequest);

            // Execute the request through the resilience pipeline
            // This applies Circuit Breaker, Retry, and Timeout policies automatically
            var result = await _resiliencePipeline.ExecuteAsync(async (ct) =>
            {
                return await CallExternalApiAsync(ct);
            }, cancellationToken);

            _logger.LogDebug(LogDebugRandomSuccess, result);
            return result;
        }
        catch (BrokenCircuitException ex)
        {
            // Circuit breaker is open - external service is considered unhealthy
            _logger.LogWarning(ex, LogWarningCircuitBreaker);
        }
        catch (TimeoutRejectedException ex)
        {
            // Request timed out after all retries
            _logger.LogError(ex, LogErrorTimeout);
        }
        catch (Exception ex)
        {
            // All other exceptions (HTTP errors, network failures, etc.)
            _logger.LogError(ex, LogErrorGeneral);
        }

        // Fallback mechanism: If resilience pipeline fails and fallback is enabled
        if (_settings.EnableFallback)
        {
            // Use Random.Shared for thread-safe random number generation in .NET 6+
            // This ensures each fallback call generates a truly random number
            var fallbackNumber = Random.Shared.Next(_settings.FallbackMinValue, _settings.FallbackMaxValue);
            _logger.LogInformation(LogInfoFallback, fallbackNumber);
            return fallbackNumber;
        }

        // Terminal failure: Both resilience pipeline failed AND fallback is disabled
        throw new InvalidOperationException("Failed to get random number from external service and fallback is disabled");
    }

    /// <summary>
    /// Builds a comprehensive resilience pipeline using Polly that combines multiple resilience patterns.
    /// The pipeline is designed to handle various failure scenarios gracefully while maintaining service availability.
    /// 
    /// Pipeline Components:
    /// 1. Timeout Policy - Prevents requests from hanging indefinitely
    /// 2. Circuit Breaker - Monitors failure rate and temporarily blocks requests to failing services
    /// 3. Retry Policy - Automatically retries failed requests with exponential backoff and jitter
    /// </summary>
    /// <returns>A configured resilience pipeline for external API calls</returns>
    private ResiliencePipeline<int> BuildResiliencePipeline()
    {
        var pipelineBuilder = new ResiliencePipelineBuilder<int>();

        // 1. Timeout Policy - Applied first to ensure overall request timeout
        pipelineBuilder.AddTimeout(new TimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds),
            OnTimeout = (args) =>
            {
                _logger.LogWarning("Request to random number service timed out after {TimeoutSeconds} seconds", _settings.TimeoutSeconds);
                return ValueTask.CompletedTask;
            }
        });

        // 2. Circuit Breaker Policy - Monitors service health and prevents cascading failures
        pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<int>
        {
            FailureRatio = _settings.CircuitBreaker.FailureRateThreshold / 100.0, // Convert percentage to ratio
            MinimumThroughput = _settings.CircuitBreaker.MinimumThroughput,
            SamplingDuration = TimeSpan.FromSeconds(_settings.CircuitBreaker.SamplingPeriodSeconds),
            BreakDuration = TimeSpan.FromSeconds(_settings.CircuitBreaker.OpenTimeoutSeconds),
            
            // Define what constitutes a failure for circuit breaker evaluation
            ShouldHandle = new PredicateBuilder<int>()
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>()
                .Handle<TimeoutException>()
                .HandleResult(result => result == -1), // Treat -1 as failure indicator
                
            OnOpened = (args) =>
            {
                _logger.LogError("Circuit breaker opened - Random number service is considered unhealthy. " +
                    "Duration: {BreakDuration}", args.BreakDuration);
                return ValueTask.CompletedTask;
            },
            
            OnClosed = (args) =>
            {
                _logger.LogInformation("Circuit breaker closed - Random number service is healthy again");
                return ValueTask.CompletedTask;
            },
            
            OnHalfOpened = (args) =>
            {
                _logger.LogInformation("Circuit breaker half-opened - Testing random number service recovery");
                return ValueTask.CompletedTask;
            }
        });

        // 3. Retry Policy - Handles transient failures with exponential backoff
        pipelineBuilder.AddRetry(new RetryStrategyOptions<int>
        {
            MaxRetryAttempts = _settings.RetryCount,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(10),
            UseJitter = true, // Add randomness to prevent thundering herd
            
            // Define what failures should trigger a retry
            ShouldHandle = new PredicateBuilder<int>()
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>()
                .HandleResult(result => result == -1), // Treat -1 as retryable failure
                
            OnRetry = (args) =>
            {
                _logger.LogWarning("Retrying request to random number service. " +
                    "Attempt: {AttemptNumber}/{MaxAttempts}, Delay: {Delay}ms", 
                    args.AttemptNumber + 1, _settings.RetryCount + 1, args.RetryDelay.TotalMilliseconds);
                return ValueTask.CompletedTask;
            }
        });

        return pipelineBuilder.Build();
    }

    /// <summary>
    /// Core method that performs the actual HTTP request to the external random number service.
    /// This method is executed within the resilience pipeline context.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request</param>
    /// <returns>Random number from external service, or -1 to indicate failure</returns>
    private async Task<int> CallExternalApiAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug(LogDebugApiRequest);
        
        var response = await _httpClient.GetAsync(Constants.ExternalServices.RandomNumberEndpoint, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(LogWarningApiStatus, response.StatusCode, response.ReasonPhrase);
            return -1;
        }
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        
        try
        {
            // Parse JSON response using efficient DTO deserialization
            var randomResponse = JsonSerializer.Deserialize<RandomNumberResponse>(content, JsonOptions);
            
            if (randomResponse?.RandomNumber > 0)
            {
                _logger.LogDebug(LogDebugApiSuccess, randomResponse.RandomNumber);
                return randomResponse.RandomNumber;
            }
            else
            {
                _logger.LogWarning(LogWarningInvalidNumber, randomResponse?.RandomNumber);
                return -1;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, LogWarningJsonParse, content);
            return -1;
        }
    }

    /// <summary>
    /// Proper disposal implementation.
    /// </summary>
    public void Dispose()
    {
        // HttpClient is managed by HttpClientFactory, so no manual disposal needed
        // ResiliencePipeline is also managed by the framework
        GC.SuppressFinalize(this);
    }
}