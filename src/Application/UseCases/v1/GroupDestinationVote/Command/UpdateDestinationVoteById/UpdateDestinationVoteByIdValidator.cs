using FluentValidation;

namespace Application.UseCases.v1.GroupDestinationVote.Command.UpdateDestinationVoteById;

public class UpdateDestinationVoteByIdValidator : AbstractValidator<UpdateDestinationVoteByIdCommand>
{
    public UpdateDestinationVoteByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.DestinationVoteId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
        
        RuleFor(x => x.IsApproved)
            .NotNull();
    }
}
