using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Services;

/// <summary>
/// Core game logic service implementing Rock, Paper, Scissors, Lizard, Spock rules.
/// Game Rules:
/// - Rock crushes Lizard and Scissors
/// - Paper covers Rock and disproves Spock
/// - Scissors cuts Paper and decapitates Lizard
/// - Lizard eats Paper and poisons Spock
/// - Spock vaporizes Rock and smashes Scissors
/// </summary>
public class GameLogicService : IGameLogicService
{
    private static readonly IReadOnlyDictionary<ChoiceType, IReadOnlyList<ChoiceType>> WinConditions = 
        new Dictionary<ChoiceType, IReadOnlyList<ChoiceType>>
        {
            // Rock wins against Lizard (crushes) and Scissors (crushes)
            { ChoiceType.Rock, new[] { ChoiceType.Lizard, ChoiceType.Scissors } },
            
            // Paper wins against Rock (covers) and Spock (disproves)
            { ChoiceType.Paper, new[] { ChoiceType.Rock, ChoiceType.Spock } },
            
            // Scissors wins against Paper (cuts) and Lizard (decapitates)
            { ChoiceType.Scissors, new[] { ChoiceType.Paper, ChoiceType.Lizard } },
            
            // Lizard wins against Spock (poisons) and Paper (eats)
            { ChoiceType.Lizard, new[] { ChoiceType.Spock, ChoiceType.Paper } },
            
            // Spock wins against Scissors (smashes) and Rock (vaporizes)
            { ChoiceType.Spock, new[] { ChoiceType.Scissors, ChoiceType.Rock } }
        };

    /// <summary>
    /// Determines the winner of a single game round by comparing player and computer choices.
    /// This method is the primary entry point for game logic evaluation.
    /// </summary>
    /// <param name="playerChoice">The choice made by the human player</param>
    /// <param name="computerChoice">The choice made by the computer opponent</param>
    /// <returns>
    /// A GameResult containing both choices and the outcome from the player's perspective
    /// (Win = player wins, Lose = computer wins, Tie = draw)
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when either choice parameter is null</exception>
    public GameResult DetermineWinner(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice == null) throw new ArgumentNullException(nameof(playerChoice));
        if (computerChoice == null) throw new ArgumentNullException(nameof(computerChoice));

        var outcome = CalculateOutcome(playerChoice, computerChoice);
        
        return new GameResult(playerChoice, computerChoice, outcome);
    }

    /// <summary>
    /// Retrieves all available game choices for display in the user interface.
    /// This method provides a consistent way to access the complete set of valid choices.
    /// </summary>
    /// <returns>An immutable list of all possible game choices</returns>
    public IReadOnlyList<Choice> GetAllChoices()
    {
        return Choice.GetAll();
    }

    /// <summary>
    /// Core logic determining game outcome based on RPSLS rules.
    /// This method implements the actual game logic using efficient lookup operations.
    /// 
    /// 1. Check for tie condition (same choices)
    /// 2. Use win conditions lookup to determine if player wins
    /// 3. If player doesn't win and it's not a tie, player loses
    /// </summary>
    /// <param name="playerChoice">The player's chosen option</param>
    /// <param name="computerChoice">The computer's chosen option</param>
    /// <returns>Game outcome from the player's perspective</returns>
    private static GameOutcome CalculateOutcome(Choice playerChoice, Choice computerChoice)
    {
        // This check must come first to avoid unnecessary lookup operations
        if (playerChoice.Type == computerChoice.Type)
        {
            return GameOutcome.Tie;
        }

        return WinConditions[playerChoice.Type].Contains(computerChoice.Type)
            ? GameOutcome.Win
            : GameOutcome.Lose;
    }
}