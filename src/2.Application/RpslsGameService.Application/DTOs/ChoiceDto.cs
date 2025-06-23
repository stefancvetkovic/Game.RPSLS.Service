using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RpslsGameService.Application.DTOs;

/// <summary>
/// Data transfer object representing a game choice option.
/// </summary>
public sealed record ChoiceDto : IEquatable<ChoiceDto>
{
    /// <summary>
    /// Unique identifier for the choice (1-5).
    /// </summary>
    [Range(1, 5, ErrorMessage = "Choice ID must be between 1 and 5")]
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Display name of the choice (Rock, Paper, Scissors, Lizard, Spock).
    /// </summary>
    [Required(ErrorMessage = "Choice name is required")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "Choice name must be between 4 and 10 characters")]
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Custom equality comparison for better performance.
    /// </summary>
    public bool Equals(ChoiceDto? other)
    {
        return other is not null && Id == other.Id && Name == other.Name;
    }

    /// <summary>
    /// Custom hash code for better performance in collections.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}