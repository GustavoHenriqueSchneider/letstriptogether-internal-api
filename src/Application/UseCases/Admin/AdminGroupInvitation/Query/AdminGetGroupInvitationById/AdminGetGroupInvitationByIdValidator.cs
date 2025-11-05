using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdValidator : AbstractValidator<AdminGetGroupInvitationByIdQuery>
{
    public AdminGetGroupInvitationByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.InvitationId)
            .NotEmpty().WithMessage("InvitationId is required");
    }
}
