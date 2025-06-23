using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Entities;

public class GameRound : Entity
{
    public Choice PlayerChoice { get; private set; }
    public Choice ComputerChoice { get; private set; }
    public GameOutcome Outcome { get; private set; }
    public DateTime PlayedAt { get; private set; }

    private GameRound() { }

    public GameRound(Choice playerChoice, Choice computerChoice, GameOutcome outcome) : base()
    {
        PlayerChoice = playerChoice ?? throw new ArgumentNullException(nameof(playerChoice));
        ComputerChoice = computerChoice ?? throw new ArgumentNullException(nameof(computerChoice));
        Outcome = outcome;
        PlayedAt = DateTime.UtcNow;
    }

    public static GameRound Create(Choice playerChoice, Choice computerChoice, GameOutcome outcome)
    {
        return new GameRound(playerChoice, computerChoice, outcome);
    }

    public GameResult ToGameResult()
    {
        return new GameResult(PlayerChoice, ComputerChoice, Outcome);
    }
}