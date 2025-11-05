using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

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
