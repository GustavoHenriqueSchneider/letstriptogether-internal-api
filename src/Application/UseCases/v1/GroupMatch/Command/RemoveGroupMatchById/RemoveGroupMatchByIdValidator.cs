using FluentValidation;

namespace Application.UseCases.v1.GroupMatch.Command.RemoveGroupMatchById;

public class RemoveGroupMatchByIdValidator : AbstractValidator<RemoveGroupMatchByIdCommand>
{
    public RemoveGroupMatchByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MatchId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
