using FluentValidation;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.v1.User.Command.UpdateCurrentUser;

public class UpdateCurrentUserValidator : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(UserModel.NameMaxLength);
    }
}
