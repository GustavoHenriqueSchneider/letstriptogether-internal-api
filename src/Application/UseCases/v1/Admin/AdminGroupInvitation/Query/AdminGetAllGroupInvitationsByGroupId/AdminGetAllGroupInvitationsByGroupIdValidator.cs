using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

public class AdminGetAllGroupInvitationsByGroupIdValidator : AbstractValidator<AdminGetAllGroupInvitationsByGroupIdQuery>
{
    public AdminGetAllGroupInvitationsByGroupIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
