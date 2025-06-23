using FluentValidation;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Domain;

namespace RpslsGameService.Application.Validators;

public class PlayGameRequestValidator : AbstractValidator<PlayGameRequest>
{
    public PlayGameRequestValidator()
    {
        RuleFor(x => x.Player)
            .InclusiveBetween(1, 5)
            .WithMessage(Constants.ValidationMessages.PlayerChoiceRange);
    }
}