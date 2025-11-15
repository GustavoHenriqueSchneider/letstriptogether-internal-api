using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

public class AdminGetAllGroupInvitationsByGroupIdHandler(
    IGroupInvitationRepository groupInvitationRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<AdminGetAllGroupInvitationsByGroupIdQuery, AdminGetAllGroupInvitationsByGroupIdResponse>
{
    public async Task<AdminGetAllGroupInvitationsByGroupIdResponse> Handle(AdminGetAllGroupInvitationsByGroupIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (invitations, hits) = 
            await groupInvitationRepository.GetByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllGroupInvitationsByGroupIdResponse
        {
            Data = invitations.Select(x => new AdminGetAllGroupInvitationsByGroupIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
