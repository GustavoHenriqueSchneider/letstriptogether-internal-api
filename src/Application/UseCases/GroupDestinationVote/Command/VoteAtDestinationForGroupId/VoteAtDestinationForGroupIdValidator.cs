using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

public class VoteAtDestinationForGroupIdValidator : AbstractValidator<VoteAtDestinationForGroupIdCommand>
{
    public VoteAtDestinationForGroupIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.DestinationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
        
        RuleFor(x => x.IsApproved)
            .NotNull();
    }
}
