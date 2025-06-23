namespace RpslsGameService.Application.Interfaces;

public interface IRandomNumberService
{
    Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default);
}