using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Query.GetCurrentUser;

public class GetCurrentUserValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
