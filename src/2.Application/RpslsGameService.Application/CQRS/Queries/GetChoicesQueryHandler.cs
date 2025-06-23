using AutoMapper;
using MediatR;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Domain.Interfaces;

namespace RpslsGameService.Application.CQRS.Queries;

public class GetChoicesQueryHandler : IRequestHandler<GetChoicesQuery, IReadOnlyList<ChoiceDto>>
{
    private readonly IGameLogicService _gameLogicService;
    private readonly IMapper _mapper;

    public GetChoicesQueryHandler(IGameLogicService gameLogicService, IMapper mapper)
    {
        _gameLogicService = gameLogicService ?? throw new ArgumentNullException(nameof(gameLogicService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public Task<IReadOnlyList<ChoiceDto>> Handle(GetChoicesQuery request, CancellationToken cancellationToken)
    {
        var choices = _gameLogicService.GetAllChoices();
        var choiceDtos = _mapper.Map<IReadOnlyList<ChoiceDto>>(choices);
        
        return Task.FromResult(choiceDtos);
    }
}