namespace RpslsGameService.Domain.Exceptions;

public class GameLogicException : DomainException
{
    public GameLogicException(string message) 
        : base(message)
    {
    }

    public GameLogicException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}