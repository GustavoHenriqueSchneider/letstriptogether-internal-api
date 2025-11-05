using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdValidator : AbstractValidator<AdminGetGroupByIdQuery>
{
    public AdminGetGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");
    }
}
