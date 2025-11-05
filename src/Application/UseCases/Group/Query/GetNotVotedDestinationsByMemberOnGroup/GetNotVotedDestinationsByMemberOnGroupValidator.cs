using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupValidator : AbstractValidator<GetNotVotedDestinationsByMemberOnGroupQuery>
{
    public GetNotVotedDestinationsByMemberOnGroupValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}
