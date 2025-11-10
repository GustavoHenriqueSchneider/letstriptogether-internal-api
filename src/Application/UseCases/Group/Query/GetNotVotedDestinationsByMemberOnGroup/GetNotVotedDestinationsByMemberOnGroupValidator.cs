using FluentValidation;

namespace Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupValidator : AbstractValidator<GetNotVotedDestinationsByMemberOnGroupQuery>
{
    public GetNotVotedDestinationsByMemberOnGroupValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
