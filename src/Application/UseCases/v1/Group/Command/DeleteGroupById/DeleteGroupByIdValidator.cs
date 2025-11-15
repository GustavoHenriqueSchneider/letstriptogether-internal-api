using FluentValidation;

namespace Application.UseCases.v1.Group.Command.DeleteGroupById;

public class DeleteGroupByIdValidator : AbstractValidator<DeleteGroupByIdCommand>
{
    public DeleteGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
