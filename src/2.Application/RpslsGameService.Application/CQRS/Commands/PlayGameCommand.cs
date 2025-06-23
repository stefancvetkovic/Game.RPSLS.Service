using MediatR;
using RpslsGameService.Application.DTOs;

namespace RpslsGameService.Application.CQRS.Commands;

public record PlayGameCommand(int PlayerChoice) : IRequest<GameResultResponse>;