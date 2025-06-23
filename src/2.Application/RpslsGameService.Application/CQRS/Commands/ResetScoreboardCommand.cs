using MediatR;

namespace RpslsGameService.Application.CQRS.Commands;

public record ResetScoreboardCommand : IRequest;