using FluentValidation;

namespace Application.UseCases.v1.Invitation.Query.GetInvitationDetails;

public class GetInvitationDetailsValidator : AbstractValidator<GetInvitationDetailsQuery>
{
    public GetInvitationDetailsValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}
