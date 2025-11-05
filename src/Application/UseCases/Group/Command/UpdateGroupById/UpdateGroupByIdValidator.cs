using FluentValidation;
using GroupModel = LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.UpdateGroupById;

public class UpdateGroupByIdValidator : AbstractValidator<UpdateGroupByIdCommand>
{
    public UpdateGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(GroupModel.NameMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.TripExpectedDate)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.TripExpectedDate.HasValue);
    }
}
