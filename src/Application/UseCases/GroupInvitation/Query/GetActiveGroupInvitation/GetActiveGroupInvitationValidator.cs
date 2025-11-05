using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;

public class GetActiveGroupInvitationValidator : AbstractValidator<GetActiveGroupInvitationQuery>
{
    public GetActiveGroupInvitationValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
