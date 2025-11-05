using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

public class AdminGetGroupDestinationVoteByIdValidator : AbstractValidator<AdminGetGroupDestinationVoteByIdQuery>
{
    public AdminGetGroupDestinationVoteByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.DestinationVoteId)
            .NotEmpty().WithMessage("DestinationVoteId is required");
    }
}
