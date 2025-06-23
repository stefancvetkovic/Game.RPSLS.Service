using RpslsGameService.Domain.Enums;

namespace RpslsGameService.Domain.Models;

public sealed class GameResult
{
    public Choice PlayerChoice { get; }
    public Choice ComputerChoice { get; }
    public GameOutcome Outcome { get; }
    public string ResultMessage { get; }
    public DateTime PlayedAt { get; }

    public GameResult(Choice playerChoice, Choice computerChoice, GameOutcome outcome)
    {
        PlayerChoice = playerChoice ?? throw new ArgumentNullException(nameof(playerChoice));
        ComputerChoice = computerChoice ?? throw new ArgumentNullException(nameof(computerChoice));
        Outcome = outcome;
        ResultMessage = GenerateResultMessage(playerChoice, computerChoice, outcome);
        PlayedAt = DateTime.UtcNow;
    }

    private static string GenerateResultMessage(Choice playerChoice, Choice computerChoice, GameOutcome outcome)
    {
        if (outcome == GameOutcome.Tie)
        {
            return string.Format(Constants.GameMessages.TieMessage, playerChoice.Name);
        }

        var verb = GetVerbForChoices(playerChoice, computerChoice);
        
        return outcome == GameOutcome.Win
            ? string.Format(Constants.GameMessages.WinMessage, playerChoice.Name, verb, computerChoice.Name)
            : string.Format(Constants.GameMessages.LoseMessage, computerChoice.Name, verb, playerChoice.Name);
    }

    private static string GetVerbForChoices(Choice choice1, Choice choice2)
    {
        return (choice1.Type, choice2.Type) switch
        {
            (ChoiceType.Rock, ChoiceType.Scissors) => Constants.GameActions.Crushes,
            (ChoiceType.Rock, ChoiceType.Lizard) => Constants.GameActions.Crushes,
            (ChoiceType.Paper, ChoiceType.Rock) => Constants.GameActions.Covers,
            (ChoiceType.Paper, ChoiceType.Spock) => Constants.GameActions.Disproves,
            (ChoiceType.Scissors, ChoiceType.Paper) => Constants.GameActions.Cuts,
            (ChoiceType.Scissors, ChoiceType.Lizard) => Constants.GameActions.Decapitates,
            (ChoiceType.Lizard, ChoiceType.Spock) => Constants.GameActions.Poisons,
            (ChoiceType.Lizard, ChoiceType.Paper) => Constants.GameActions.Eats,
            (ChoiceType.Spock, ChoiceType.Scissors) => Constants.GameActions.Smashes,
            (ChoiceType.Spock, ChoiceType.Rock) => Constants.GameActions.Vaporizes,
            _ => Constants.GameActions.Defeats
        };
    }
}