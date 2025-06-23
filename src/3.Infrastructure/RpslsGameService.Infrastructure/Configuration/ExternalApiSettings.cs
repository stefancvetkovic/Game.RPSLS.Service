using RpslsGameService.Domain;

namespace RpslsGameService.Infrastructure.Configuration;

public class ExternalApiSettings
{
    public const string SectionName = Constants.ConfigurationSections.ExternalApis;
    
    public RandomNumberServiceSettings RandomNumberService { get; set; } = new();
}

public class RandomNumberServiceSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
    public int RetryCount { get; set; }
    public bool EnableFallback { get; set; }
    
    /// <summary>
    /// Minimum value for fallback random number generation.
    /// Default: 1
    /// </summary>
    public int FallbackMinValue { get; set; } = 1;
    
    /// <summary>
    /// Maximum value for fallback random number generation.
    /// Default: 101 (exclusive upper bound)
    /// </summary>
    public int FallbackMaxValue { get; set; } = 101;
    
    // Circuit Breaker Configuration
    public CircuitBreakerSettings CircuitBreaker { get; set; } = new();
}

/// <summary>
/// Configuration settings for the Circuit Breaker pattern implementation.
/// The Circuit Breaker prevents cascading failures by monitoring the failure rate
/// of external service calls and temporarily stopping requests when thresholds are exceeded.
/// More on https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern
/// </summary>
public class CircuitBreakerSettings
{
    /// <summary>
    /// Time in seconds to keep the circuit breaker in the Open state
    /// before transitioning to Half-Open to test service recovery.
    /// </summary>
    public int OpenTimeoutSeconds { get; set; }
    
    /// <summary>
    /// Minimum number of requests required in the sampling period
    /// before the circuit breaker can evaluate the failure rate.
    /// </summary>
    public int MinimumThroughput { get; set; }
    
    /// <summary>
    /// Time window in seconds for sampling failure rate.
    /// The circuit breaker evaluates failures within this rolling window.
    /// </summary>
    public int SamplingPeriodSeconds { get; set; }
    
    /// <summary>
    /// Failure rate percentage (0-100) that triggers the circuit breaker to open.
    /// If failure rate exceeds this percentage within the sampling period,
    /// the circuit breaker opens and blocks subsequent requests.
    /// </summary>
    public double FailureRateThreshold { get; set; }
}