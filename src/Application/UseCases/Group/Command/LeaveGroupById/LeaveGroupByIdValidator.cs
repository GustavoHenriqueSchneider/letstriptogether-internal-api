using FluentValidation;

namespace Application.UseCases.Group.Command.LeaveGroupById;

public class LeaveGroupByIdValidator : AbstractValidator<LeaveGroupByIdCommand>
{
    public LeaveGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
