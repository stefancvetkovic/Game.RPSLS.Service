using MediatR;
using RpslsGameService.Application.DTOs;

namespace RpslsGameService.Application.CQRS.Queries;

public record GetRandomChoiceQuery : IRequest<ChoiceDto>;