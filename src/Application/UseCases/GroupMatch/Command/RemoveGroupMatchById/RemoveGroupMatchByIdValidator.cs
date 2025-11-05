using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;

public class RemoveGroupMatchByIdValidator : AbstractValidator<RemoveGroupMatchByIdCommand>
{
    public RemoveGroupMatchByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("MatchId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
