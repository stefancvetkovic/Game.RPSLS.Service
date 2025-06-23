namespace RpslsGameService.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for HTTP client optimization.
/// </summary>
public class HttpClientSettings
{
    public const string SectionName = "HttpClientSettings";

    /// <summary>
    /// Maximum number of connections per server.
    /// Default: 10
    /// </summary>
    public int MaxConnectionsPerServer { get; set; } = 10;

    /// <summary>
    /// Connection lifetime in minutes.
    /// Default: 5 minutes
    /// </summary>
    public int ConnectionLifetimeMinutes { get; set; } = 5;

    /// <summary>
    /// Enable response compression.
    /// Default: true
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Use cookies for HTTP requests.
    /// Default: false (not needed for API calls)
    /// </summary>
    public bool UseCookies { get; set; } = false;
}