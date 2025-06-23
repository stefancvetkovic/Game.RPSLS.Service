using Microsoft.Extensions.Logging;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Domain.Entities;

namespace RpslsGameService.Infrastructure.Persistence;

public class InMemoryGameSessionRepository : IGameSessionRepository
{
    private readonly ILogger<InMemoryGameSessionRepository> _logger;
    private GameSession? _currentSession;
    private readonly object _lock = new();

    public InMemoryGameSessionRepository(ILogger<InMemoryGameSessionRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<GameSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_currentSession?.Id == id)
            {
                _logger.LogDebug("Found game session with ID: {SessionId}", id);
                return Task.FromResult<GameSession?>(_currentSession);
            }

            _logger.LogDebug("Game session with ID {SessionId} not found", id);
            return Task.FromResult<GameSession?>(null);
        }
    }

    public Task<GameSession> SaveAsync(GameSession gameSession, CancellationToken cancellationToken = default)
    {
        if (gameSession == null) throw new ArgumentNullException(nameof(gameSession));

        lock (_lock)
        {
            _currentSession = gameSession;
            _logger.LogDebug("Saved game session with ID: {SessionId}, Total rounds: {TotalRounds}", 
                gameSession.Id, gameSession.TotalRounds);
            
            return Task.FromResult(gameSession);
        }
    }

    public Task<GameSession> GetCurrentSessionAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_currentSession == null)
            {
                _currentSession = GameSession.Create();
                _logger.LogDebug("Created new game session with ID: {SessionId}", _currentSession.Id);
            }
            else
            {
                _logger.LogDebug("Retrieved existing game session with ID: {SessionId}", _currentSession.Id);
            }

            return Task.FromResult(_currentSession);
        }
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_currentSession != null)
            {
                _logger.LogInformation("Resetting game session with ID: {SessionId}", _currentSession.Id);
                _currentSession.Reset();
            }
            else
            {
                _logger.LogDebug("No current session to reset");
            }

            return Task.CompletedTask;
        }
    }
}