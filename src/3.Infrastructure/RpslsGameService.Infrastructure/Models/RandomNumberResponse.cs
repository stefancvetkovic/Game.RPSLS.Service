using System.Text.Json.Serialization;

namespace RpslsGameService.Infrastructure.Models;

/// <summary>
/// Response model for external random number service.
/// </summary>
public sealed class RandomNumberResponse
{
    /// <summary>
    /// The random number returned by the external service.
    /// </summary>
    [JsonPropertyName("random_number")]
    public int RandomNumber { get; set; }
}