namespace RpslsGameService.Domain.Exceptions;

public class InvalidChoiceException : DomainException
{
    public int? InvalidId { get; }

    public InvalidChoiceException(int id) 
        : base(string.Format(Constants.ValidationMessages.InvalidChoiceId, id))
    {
        InvalidId = id;
    }

    public InvalidChoiceException(string message) 
        : base(message)
    {
    }
}