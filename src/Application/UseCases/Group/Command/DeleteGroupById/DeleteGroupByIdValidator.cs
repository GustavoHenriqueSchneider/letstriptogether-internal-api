using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.DeleteGroupById;

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
