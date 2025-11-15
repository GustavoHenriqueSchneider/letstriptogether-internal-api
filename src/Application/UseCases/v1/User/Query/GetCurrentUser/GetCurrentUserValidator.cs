using FluentValidation;

namespace Application.UseCases.v1.User.Query.GetCurrentUser;

public class GetCurrentUserValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
