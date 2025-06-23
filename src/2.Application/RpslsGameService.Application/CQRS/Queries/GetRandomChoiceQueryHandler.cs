using AutoMapper;
using MediatR;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Domain.Interfaces;

namespace RpslsGameService.Application.CQRS.Queries;

public class GetRandomChoiceQueryHandler : IRequestHandler<GetRandomChoiceQuery, ChoiceDto>
{
    private readonly IChoiceGenerationService _choiceGenerationService;
    private readonly IRandomNumberService _randomNumberService;
    private readonly IMapper _mapper;

    public GetRandomChoiceQueryHandler(
        IChoiceGenerationService choiceGenerationService,
        IRandomNumberService randomNumberService,
        IMapper mapper)
    {
        _choiceGenerationService = choiceGenerationService ?? throw new ArgumentNullException(nameof(choiceGenerationService));
        _randomNumberService = randomNumberService ?? throw new ArgumentNullException(nameof(randomNumberService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ChoiceDto> Handle(GetRandomChoiceQuery request, CancellationToken cancellationToken)
    {
        var randomNumber = await _randomNumberService.GetRandomNumberAsync(cancellationToken);
        var choice = _choiceGenerationService.GenerateComputerChoice(randomNumber);
        
        return _mapper.Map<ChoiceDto>(choice);
    }
}