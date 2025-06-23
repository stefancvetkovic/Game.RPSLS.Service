using System.Text.Json.Serialization;

namespace RpslsGameService.Application.DTOs;

/// <summary>
/// Standardized error response model for consistent API error handling.
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Human-readable error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Error code for programmatic handling.
    /// </summary>
    [JsonPropertyName("error_code")]
    public string ErrorCode { get; init; } = string.Empty;

    /// <summary>
    /// Additional details about the error (optional).
    /// </summary>
    [JsonPropertyName("details")]
    public object? Details { get; init; }

    /// <summary>
    /// Timestamp when the error occurred.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Request ID for correlation (optional).
    /// </summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; }
}