using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

public class AdminGetGroupDestinationVoteByIdValidator : AbstractValidator<AdminGetGroupDestinationVoteByIdQuery>
{
    public AdminGetGroupDestinationVoteByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.DestinationVoteId)
            .NotEmpty();
    }
}
