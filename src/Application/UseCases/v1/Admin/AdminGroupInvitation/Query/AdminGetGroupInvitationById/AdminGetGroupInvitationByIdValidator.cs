using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdValidator : AbstractValidator<AdminGetGroupInvitationByIdQuery>
{
    public AdminGetGroupInvitationByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.InvitationId)
            .NotEmpty();
    }
}
