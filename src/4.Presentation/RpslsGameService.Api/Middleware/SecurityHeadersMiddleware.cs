namespace RpslsGameService.Api.Middleware;

/// <summary>
/// Middleware to add security headers to HTTP responses
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers before processing the request
        AddSecurityHeaders(context);

        await _next(context);
    }

    private static void AddSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Remove server information
        headers.Remove("Server");

        // X-Content-Type-Options: Prevents MIME type sniffing
        if (!headers.ContainsKey("X-Content-Type-Options"))
        {
            headers.Add("X-Content-Type-Options", "nosniff");
        }

        // X-Frame-Options: Prevents clickjacking attacks
        if (!headers.ContainsKey("X-Frame-Options"))
        {
            headers.Add("X-Frame-Options", "DENY");
        }

        // X-XSS-Protection: Enables XSS filtering
        if (!headers.ContainsKey("X-XSS-Protection"))
        {
            headers.Add("X-XSS-Protection", "1; mode=block");
        }

        // Referrer-Policy: Controls how much referrer information is included
        if (!headers.ContainsKey("Referrer-Policy"))
        {
            headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        }

        // Content-Security-Policy: Helps prevent XSS attacks
        if (!headers.ContainsKey("Content-Security-Policy"))
        {
            var csp = "default-src 'self'; " +
                     "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                     "style-src 'self' 'unsafe-inline'; " +
                     "img-src 'self' data: https:; " +
                     "connect-src 'self'; " +
                     "font-src 'self'; " +
                     "object-src 'none'; " +
                     "media-src 'self'; " +
                     "frame-src 'none';";
            
            headers.Add("Content-Security-Policy", csp);
        }

        // Strict-Transport-Security: Enforces HTTPS (only add if using HTTPS)
        if (context.Request.IsHttps && !headers.ContainsKey("Strict-Transport-Security"))
        {
            headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        }

        // X-Permitted-Cross-Domain-Policies: Restricts cross-domain policies
        if (!headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
        {
            headers.Add("X-Permitted-Cross-Domain-Policies", "none");
        }

        // Feature-Policy/Permissions-Policy: Controls which browser features can be used
        if (!headers.ContainsKey("Permissions-Policy"))
        {
            var permissionsPolicy = "geolocation=(), " +
                                  "microphone=(), " +
                                  "camera=(), " +
                                  "fullscreen=(self), " +
                                  "payment=()";
            
            headers.Add("Permissions-Policy", permissionsPolicy);
        }

        // X-DNS-Prefetch-Control: Controls DNS prefetching
        if (!headers.ContainsKey("X-DNS-Prefetch-Control"))
        {
            headers.Add("X-DNS-Prefetch-Control", "off");
        }

        // Cache-Control for API responses (prevent caching of sensitive data)
        if (context.Request.Path.StartsWithSegments("/api") && 
            !headers.ContainsKey("Cache-Control"))
        {
            headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            headers.Add("Pragma", "no-cache");
            headers.Add("Expires", "0");
        }

        // Add custom security headers for API
        if (!headers.ContainsKey("X-API-Version"))
        {
            headers.Add("X-API-Version", "1.0");
        }

        if (!headers.ContainsKey("X-Powered-By"))
        {
            headers.Remove("X-Powered-By"); // Remove this header entirely for security
        }
    }
}