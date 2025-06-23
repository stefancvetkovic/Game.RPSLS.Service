using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Entities;

public class GameSession : Entity
{
    private readonly List<GameRound> _rounds = new();
    
    public IReadOnlyList<GameRound> Rounds => _rounds.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastPlayedAt { get; private set; }
    public int TotalRounds => _rounds.Count;
    public int PlayerWins => _rounds.Count(r => r.Outcome == Enums.GameOutcome.Win);
    public int ComputerWins => _rounds.Count(r => r.Outcome == Enums.GameOutcome.Lose);
    public int Ties => _rounds.Count(r => r.Outcome == Enums.GameOutcome.Tie);

    private GameSession() { }

    public GameSession(Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static GameSession Create()
    {
        return new GameSession();
    }

    public GameResult PlayRound(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice == null) throw new ArgumentNullException(nameof(playerChoice));
        if (computerChoice == null) throw new ArgumentNullException(nameof(computerChoice));

        var outcome = DetermineOutcome(playerChoice, computerChoice);
        var round = GameRound.Create(playerChoice, computerChoice, outcome);
        
        _rounds.Add(round);
        LastPlayedAt = DateTime.UtcNow;

        return round.ToGameResult();
    }

    private static Enums.GameOutcome DetermineOutcome(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice.Type == computerChoice.Type)
        {
            return Enums.GameOutcome.Tie;
        }

        var winConditions = GetWinConditions();
        
        return winConditions[playerChoice.Type].Contains(computerChoice.Type) 
            ? Enums.GameOutcome.Win 
            : Enums.GameOutcome.Lose;
    }

    private static IReadOnlyDictionary<Enums.ChoiceType, IReadOnlyList<Enums.ChoiceType>> GetWinConditions()
    {
        return new Dictionary<Enums.ChoiceType, IReadOnlyList<Enums.ChoiceType>>
        {
            { Enums.ChoiceType.Rock, new[] { Enums.ChoiceType.Lizard, Enums.ChoiceType.Scissors } },
            { Enums.ChoiceType.Paper, new[] { Enums.ChoiceType.Rock, Enums.ChoiceType.Spock } },
            { Enums.ChoiceType.Scissors, new[] { Enums.ChoiceType.Paper, Enums.ChoiceType.Lizard } },
            { Enums.ChoiceType.Lizard, new[] { Enums.ChoiceType.Spock, Enums.ChoiceType.Paper } },
            { Enums.ChoiceType.Spock, new[] { Enums.ChoiceType.Scissors, Enums.ChoiceType.Rock } }
        };
    }

    public void Reset()
    {
        _rounds.Clear();
        LastPlayedAt = null;
    }
}