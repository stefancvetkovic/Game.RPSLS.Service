namespace RpslsGameService.Application.DTOs;

public record GameHistoryResponse
{
    public IReadOnlyList<GameResultResponse> History { get; init; } = Array.Empty<GameResultResponse>();
    public int TotalGames { get; init; }
    public int PlayerWins { get; init; }
    public int ComputerWins { get; init; }
    public int Ties { get; init; }
}