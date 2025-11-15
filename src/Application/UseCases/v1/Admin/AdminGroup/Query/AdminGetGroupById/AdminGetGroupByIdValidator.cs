using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdValidator : AbstractValidator<AdminGetGroupByIdQuery>
{
    public AdminGetGroupByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}
