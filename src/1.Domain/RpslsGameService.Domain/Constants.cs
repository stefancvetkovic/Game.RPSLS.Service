namespace RpslsGameService.Domain;

/// <summary>
/// Domain constants for the RPSLS Game Service.
/// </summary>
public static class Constants
{
    /// <summary>
    /// External service configuration constants.
    /// </summary>
    public static class ExternalServices
    {
        public const string RandomNumberEndpoint = "/random";
    }

    /// <summary>
    /// Configuration section names.
    /// </summary>
    public static class ConfigurationSections
    {
        public const string ExternalApis = "ExternalApis";
        public const string ApiKeySettings = "ApiKeySettings";
        public const string ApplicationSettings = "ApplicationSettings";
        public const string Caching = "Caching";
    }

    /// <summary>
    /// Validation error messages.
    /// </summary>
    public static class ValidationMessages
    {
        public const string InvalidChoiceId = "Invalid choice ID: {0}. Valid IDs are 1-5.";
        public const string InvalidChoiceIdGeneric = "Invalid choice id: {0}";
        public const string InvalidChoiceType = "Invalid choice type: {0}";
        public const string PlayerChoiceRange = "Player choice must be between 1 and 5 (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock)";
        public const string RequiredParameterMissing = "Required parameter is missing";
        public const string InternalServerError = "An internal server error occurred";
        public const string TooManyRequests = "Too many requests. Please try again later.";
    }

    /// <summary>
    /// API route names.
    /// </summary>
    public static class ApiRoutes
    {
        public const string Choices = "choices";
        public const string Choice = "choice";
        public const string Play = "play";
        public const string History = "history";
        public const string Reset = "reset";
    }

    /// <summary>
    /// Game result message templates.
    /// </summary>
    public static class GameMessages
    {
        public const string TieMessage = "Both players chose {0}. It's a tie!";
        public const string WinMessage = "{0} {1} {2}. You win!";
        public const string LoseMessage = "{0} {1} {2}. You lose!";
    }

    /// <summary>
    /// Game outcome strings.
    /// </summary>
    public static class GameOutcomes
    {
        public const string Win = "win";
        public const string Lose = "lose";
        public const string Tie = "tie";
        public const string Unknown = "unknown";
    }

    /// <summary>
    /// Game action verbs.
    /// </summary>
    public static class GameActions
    {
        public const string Crushes = "crushes";
        public const string Covers = "covers";
        public const string Disproves = "disproves";
        public const string Cuts = "cuts";
        public const string Decapitates = "decapitates";
        public const string Poisons = "poisons";
        public const string Eats = "eats";
        public const string Smashes = "smashes";
        public const string Vaporizes = "vaporizes";
        public const string Defeats = "defeats";
    }

    /// <summary>
    /// Game choice names.
    /// </summary>
    public static class GameChoices
    {
        public const string Rock = "Rock";
        public const string Paper = "Paper";
        public const string Scissors = "Scissors";
        public const string Lizard = "Lizard";
        public const string Spock = "Spock";
    }
}