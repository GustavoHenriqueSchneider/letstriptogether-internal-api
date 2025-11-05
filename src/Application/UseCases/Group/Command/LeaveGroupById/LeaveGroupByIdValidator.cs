using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.LeaveGroupById;

public class LeaveGroupByIdValidator : AbstractValidator<LeaveGroupByIdCommand>
{
    public LeaveGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
