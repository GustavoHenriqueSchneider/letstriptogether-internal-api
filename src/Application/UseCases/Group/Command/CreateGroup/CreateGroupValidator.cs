using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;

public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.TripExpectedDate)
            .NotEmpty().WithMessage("TripExpectedDate is required");
    }
}
