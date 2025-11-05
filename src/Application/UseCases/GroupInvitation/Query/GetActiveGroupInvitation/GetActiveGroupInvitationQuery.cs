using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;

public class GetActiveGroupInvitationQuery : IRequest<GetActiveGroupInvitationResponse>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
