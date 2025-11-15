using FluentValidation;

namespace Application.UseCases.v1.GroupInvitation.Query.GetActiveGroupInvitation;

public class GetActiveGroupInvitationValidator : AbstractValidator<GetActiveGroupInvitationQuery>
{
    public GetActiveGroupInvitationValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
