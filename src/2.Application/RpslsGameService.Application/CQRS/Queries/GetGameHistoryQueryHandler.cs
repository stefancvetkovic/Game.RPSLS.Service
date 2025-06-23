using AutoMapper;
using MediatR;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.Interfaces;

namespace RpslsGameService.Application.CQRS.Queries;

public class GetGameHistoryQueryHandler : IRequestHandler<GetGameHistoryQuery, GameHistoryResponse>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IMapper _mapper;

    public GetGameHistoryQueryHandler(IGameSessionRepository gameSessionRepository, IMapper mapper)
    {
        _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<GameHistoryResponse> Handle(GetGameHistoryQuery request, CancellationToken cancellationToken)
    {
        var gameSession = await _gameSessionRepository.GetCurrentSessionAsync(cancellationToken);
        
        var history = gameSession.Rounds
            .OrderByDescending(r => r.PlayedAt)
            .Select(r => r.ToGameResult())
            .ToList();

        var historyDtos = _mapper.Map<IReadOnlyList<GameResultResponse>>(history);

        return new GameHistoryResponse
        {
            History = historyDtos,
            TotalGames = gameSession.TotalRounds,
            PlayerWins = gameSession.PlayerWins,
            ComputerWins = gameSession.ComputerWins,
            Ties = gameSession.Ties
        };
    }
}