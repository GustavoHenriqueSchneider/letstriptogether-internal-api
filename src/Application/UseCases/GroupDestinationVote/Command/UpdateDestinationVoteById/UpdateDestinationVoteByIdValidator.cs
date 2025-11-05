using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

public class UpdateDestinationVoteByIdValidator : AbstractValidator<UpdateDestinationVoteByIdCommand>
{
    public UpdateDestinationVoteByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.DestinationVoteId)
            .NotEmpty().WithMessage("DestinationVoteId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
