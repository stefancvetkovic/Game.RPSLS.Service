using MediatR;
using RpslsGameService.Application.Interfaces;

namespace RpslsGameService.Application.CQRS.Commands;

public class ResetScoreboardCommandHandler : IRequestHandler<ResetScoreboardCommand>
{
    private readonly IGameSessionRepository _gameSessionRepository;

    public ResetScoreboardCommandHandler(IGameSessionRepository gameSessionRepository)
    {
        _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
    }

    public async Task Handle(ResetScoreboardCommand request, CancellationToken cancellationToken)
    {
        await _gameSessionRepository.ResetAsync(cancellationToken);
    }
}