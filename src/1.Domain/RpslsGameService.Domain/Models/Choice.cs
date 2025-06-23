using RpslsGameService.Domain.Enums;

namespace RpslsGameService.Domain.Models;

public sealed class Choice : IEquatable<Choice>
{
    public int Id { get; }
    public string Name { get; }
    public ChoiceType Type { get; }

    private Choice(int id, string name, ChoiceType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public static Choice Rock => new((int)ChoiceType.Rock, Constants.GameChoices.Rock, ChoiceType.Rock);
    public static Choice Paper => new((int)ChoiceType.Paper, Constants.GameChoices.Paper, ChoiceType.Paper);
    public static Choice Scissors => new((int)ChoiceType.Scissors, Constants.GameChoices.Scissors, ChoiceType.Scissors);
    public static Choice Lizard => new((int)ChoiceType.Lizard, Constants.GameChoices.Lizard, ChoiceType.Lizard);
    public static Choice Spock => new((int)ChoiceType.Spock, Constants.GameChoices.Spock, ChoiceType.Spock);

    public static Choice FromId(int id)
    {
        return id switch
        {
            (int)ChoiceType.Rock => Rock,
            (int)ChoiceType.Paper => Paper,
            (int)ChoiceType.Scissors => Scissors,
            (int)ChoiceType.Lizard => Lizard,
            (int)ChoiceType.Spock => Spock,
            _ => throw new ArgumentException(string.Format(Constants.ValidationMessages.InvalidChoiceIdGeneric, id), nameof(id))
        };
    }

    public static Choice FromType(ChoiceType type)
    {
        return type switch
        {
            ChoiceType.Rock => Rock,
            ChoiceType.Paper => Paper,
            ChoiceType.Scissors => Scissors,
            ChoiceType.Lizard => Lizard,
            ChoiceType.Spock => Spock,
            _ => throw new ArgumentException(string.Format(Constants.ValidationMessages.InvalidChoiceType, type), nameof(type))
        };
    }

    public static IReadOnlyList<Choice> GetAll() => new[] { Rock, Paper, Scissors, Lizard, Spock };

    public bool Equals(Choice? other)
    {
        if (other is null) return false;
        return Id == other.Id && Type == other.Type;
    }

    public override bool Equals(object? obj) => Equals(obj as Choice);

    public override int GetHashCode() => HashCode.Combine(Id, Type);

    public static bool operator ==(Choice? left, Choice? right) => Equals(left, right);

    public static bool operator !=(Choice? left, Choice? right) => !Equals(left, right);

    public override string ToString() => Name;
}