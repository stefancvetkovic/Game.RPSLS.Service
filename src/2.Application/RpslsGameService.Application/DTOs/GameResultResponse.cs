namespace RpslsGameService.Application.DTOs;

public record GameResultResponse
{
    public string Results { get; init; } = string.Empty;
    public int Player { get; init; }
    public int Computer { get; init; }
    public string PlayerChoice { get; init; } = string.Empty;
    public string ComputerChoice { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime PlayedAt { get; init; }
}