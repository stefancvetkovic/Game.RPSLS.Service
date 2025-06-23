using AutoMapper;
using MediatR;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Application.CQRS.Commands;

public class PlayGameCommandHandler : IRequestHandler<PlayGameCommand, GameResultResponse>
{
    private readonly IGameLogicService _gameLogicService;
    private readonly IChoiceGenerationService _choiceGenerationService;
    private readonly IRandomNumberService _randomNumberService;
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IMapper _mapper;

    public PlayGameCommandHandler(
        IGameLogicService gameLogicService,
        IChoiceGenerationService choiceGenerationService,
        IRandomNumberService randomNumberService,
        IGameSessionRepository gameSessionRepository,
        IMapper mapper)
    {
        _gameLogicService = gameLogicService ?? throw new ArgumentNullException(nameof(gameLogicService));
        _choiceGenerationService = choiceGenerationService ?? throw new ArgumentNullException(nameof(choiceGenerationService));
        _randomNumberService = randomNumberService ?? throw new ArgumentNullException(nameof(randomNumberService));
        _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<GameResultResponse> Handle(PlayGameCommand request, CancellationToken cancellationToken)
    {
        var playerChoice = Choice.FromId(request.PlayerChoice);
        
        var randomNumber = await _randomNumberService.GetRandomNumberAsync(cancellationToken);
        var computerChoice = _choiceGenerationService.GenerateComputerChoice(randomNumber);

        var gameResult = _gameLogicService.DetermineWinner(playerChoice, computerChoice);

        var gameSession = await _gameSessionRepository.GetCurrentSessionAsync(cancellationToken);
        gameSession.PlayRound(playerChoice, computerChoice);
        await _gameSessionRepository.SaveAsync(gameSession, cancellationToken);

        return _mapper.Map<GameResultResponse>(gameResult);
    }
}