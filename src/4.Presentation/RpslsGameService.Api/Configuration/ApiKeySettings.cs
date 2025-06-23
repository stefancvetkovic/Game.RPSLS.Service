using RpslsGameService.Domain;

namespace RpslsGameService.Api.Configuration;

/// <summary>
/// Configuration settings for API key authentication
/// </summary>
public class ApiKeySettings
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = Constants.ConfigurationSections.ApiKeySettings;

    /// <summary>
    /// Whether API key authentication is enabled
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Header name for API key
    /// </summary>
    public string HeaderName { get; set; } = "X-API-Key";

    /// <summary>
    /// List of valid API keys (in production, store in secure configuration)
    /// </summary>
    public List<string> ValidApiKeys { get; set; } = new();

    /// <summary>
    /// Whether to allow anonymous access to health endpoints
    /// </summary>
    public bool AllowAnonymousHealthChecks { get; set; } = true;

    /// <summary>
    /// List of endpoints that don't require API key authentication
    /// </summary>
    public List<string> ExemptPaths { get; set; } = new()
    {
        "/health",
        "/swagger",
        "/api/game/choices"
    };
}