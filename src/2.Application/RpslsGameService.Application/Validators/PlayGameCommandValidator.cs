using FluentValidation;
using RpslsGameService.Application.CQRS.Commands;
using RpslsGameService.Domain;

namespace RpslsGameService.Application.Validators;

public class PlayGameCommandValidator : AbstractValidator<PlayGameCommand>
{
    public PlayGameCommandValidator()
    {
        RuleFor(x => x.PlayerChoice)
            .InclusiveBetween(1, 5)
            .WithMessage(Constants.ValidationMessages.PlayerChoiceRange);
    }
}