using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.UpdateGroupById;

public class UpdateGroupByIdValidator : AbstractValidator<UpdateGroupByIdCommand>
{
    public UpdateGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
