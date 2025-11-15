using FluentValidation;

namespace Application.UseCases.v1.Group.Command.LeaveGroupById;

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
