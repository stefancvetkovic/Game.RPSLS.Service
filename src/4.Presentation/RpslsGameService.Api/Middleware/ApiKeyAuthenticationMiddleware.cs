using Microsoft.Extensions.Options;
using RpslsGameService.Api.Configuration;
using System.Net;

namespace RpslsGameService.Api.Middleware;

/// <summary>
/// Middleware for API key authentication
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeySettings _apiKeySettings;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IOptions<ApiKeySettings> apiKeySettings,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _apiKeySettings = apiKeySettings.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip API key validation if disabled
        if (!_apiKeySettings.Enabled)
        {
            await _next(context);
            return;
        }

        // Skip API key validation for exempt paths
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (_apiKeySettings.ExemptPaths.Any(exemptPath => 
            path?.StartsWith(exemptPath.ToLowerInvariant()) == true))
        {
            await _next(context);
            return;
        }

        // Check for API key in header
        if (!context.Request.Headers.TryGetValue(_apiKeySettings.HeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("API key not found in header {HeaderName} for path {Path}", 
                _apiKeySettings.HeaderName, path);
            
            await WriteUnauthorizedResponse(context, "API key is required");
            return;
        }

        var apiKey = extractedApiKey.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Empty API key provided for path {Path}", path);
            await WriteUnauthorizedResponse(context, "API key is required");
            return;
        }

        // Validate API key
        if (!_apiKeySettings.ValidApiKeys.Contains(apiKey))
        {
            _logger.LogWarning("Invalid API key provided for path {Path}. Key: {ApiKey}", 
                path, apiKey[..Math.Min(apiKey.Length, 8)] + "...");
            
            await WriteUnauthorizedResponse(context, "Invalid API key");
            return;
        }

        _logger.LogDebug("Valid API key authenticated for path {Path}", path);
        
        // Add API key to context for potential use in controllers
        context.Items["ApiKey"] = apiKey;

        await _next(context);
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                message,
                statusCode = 401,
                timestamp = DateTime.UtcNow
            }
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}