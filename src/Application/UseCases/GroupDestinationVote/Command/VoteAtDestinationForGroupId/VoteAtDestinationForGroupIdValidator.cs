using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

public class VoteAtDestinationForGroupIdValidator : AbstractValidator<VoteAtDestinationForGroupIdCommand>
{
    public VoteAtDestinationForGroupIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.DestinationId)
            .NotEmpty().WithMessage("DestinationId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
