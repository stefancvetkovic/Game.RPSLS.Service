using System.Collections.Concurrent;
using System.Net;
using RpslsGameService.Domain;

namespace RpslsGameService.Api.Middleware;

/// <summary>
/// Simple rate limiting middleware
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

    // Rate limiting configuration

    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        // x requests per time window
        int requestLimit = 100,
        // y minute(s) window
        int timeWindowMinutes = 1) 
    {
        _next = next;
        _logger = logger;
        _requestLimit = requestLimit;
        _timeWindow = TimeSpan.FromMinutes(timeWindowMinutes);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);

        // Skip rate limiting for health checks
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Check rate limit
        if (IsRateLimited(clientId))
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId} on path {Path}", 
                clientId, context.Request.Path);

            await WriteRateLimitResponse(context);
            return;
        }

        // Record the request
        RecordRequest(clientId);

        await _next(context);
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            ipAddress = forwardedFor.FirstOrDefault()?.Split(',')[0].Trim() ?? ipAddress;
        }
        else if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            ipAddress = realIp.FirstOrDefault() ?? ipAddress;
        }

        return $"ip:{ipAddress}";
    }

    private bool IsRateLimited(string clientId)
    {
        var now = DateTime.UtcNow;
        
        if (!_clients.TryGetValue(clientId, out var clientInfo))
        {
            return false;
        }

        // Clean up old requests outside the time window
        clientInfo.RequestTimes.RemoveAll(time => now - time > _timeWindow);

        // Check if client has exceeded the rate limit
        return clientInfo.RequestTimes.Count >= _requestLimit;
    }

    private void RecordRequest(string clientId)
    {
        var now = DateTime.UtcNow;
        
        _clients.AddOrUpdate(clientId,
            new ClientRequestInfo { RequestTimes = new List<DateTime> { now } },
            (key, existingInfo) =>
            {
                // Clean up old requests
                existingInfo.RequestTimes.RemoveAll(time => now - time > _timeWindow);
                existingInfo.RequestTimes.Add(now);
                return existingInfo;
            });

        // Periodic cleanup of inactive clients every minute
        if (DateTime.UtcNow.Second == 0)
        {
            CleanupInactiveClients();
        }
    }

    private void CleanupInactiveClients()
    {
        var cutoff = DateTime.UtcNow - _timeWindow;
        var clientsToRemove = _clients
            .Where(kvp => !kvp.Value.RequestTimes.Any(time => time > cutoff))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var clientId in clientsToRemove)
        {
            _clients.TryRemove(clientId, out _);
        }
    }

    private static async Task WriteRateLimitResponse(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.ContentType = "application/json";

        // Add rate limit headers
        context.Response.Headers.Add("X-RateLimit-Limit", "100");
        context.Response.Headers.Add("X-RateLimit-Remaining", "0");
        context.Response.Headers.Add("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());

        var response = new
        {
            error = new
            {
                message = Constants.ValidationMessages.TooManyRequests,
                statusCode = 429,
                timestamp = DateTime.UtcNow
            }
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private class ClientRequestInfo
    {
        public List<DateTime> RequestTimes { get; set; } = new();
    }
}