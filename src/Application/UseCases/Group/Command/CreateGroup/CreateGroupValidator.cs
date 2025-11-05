using FluentValidation;
using GroupModel = LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;

public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(GroupModel.NameMaxLength);

        RuleFor(x => x.TripExpectedDate)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);
    }
}
