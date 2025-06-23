using RpslsGameService.Domain.Entities;

namespace RpslsGameService.Application.Interfaces;

public interface IGameSessionRepository
{
    Task<GameSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GameSession> SaveAsync(GameSession gameSession, CancellationToken cancellationToken = default);
    Task<GameSession> GetCurrentSessionAsync(CancellationToken cancellationToken = default);
    Task ResetAsync(CancellationToken cancellationToken = default);
}