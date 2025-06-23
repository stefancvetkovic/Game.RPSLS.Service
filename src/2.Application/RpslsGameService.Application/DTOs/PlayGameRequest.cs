using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RpslsGameService.Application.DTOs;

/// <summary>
/// Request model for playing a game round.
/// </summary>
public sealed record PlayGameRequest
{
    /// <summary>
    /// Player's choice (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock).
    /// </summary>
    [Range(1, 5, ErrorMessage = "Player choice must be between 1 and 5 (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock)")]
    [JsonPropertyName("player")]
    public int Player { get; init; }
}