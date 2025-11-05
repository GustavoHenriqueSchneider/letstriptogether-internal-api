using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetGroupById;

public class GetGroupByIdValidator : AbstractValidator<GetGroupByIdQuery>
{
    public GetGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
