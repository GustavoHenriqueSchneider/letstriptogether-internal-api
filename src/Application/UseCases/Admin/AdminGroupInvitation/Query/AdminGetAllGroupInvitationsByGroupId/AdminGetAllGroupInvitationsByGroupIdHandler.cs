using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

public class AdminGetAllGroupInvitationsByGroupIdHandler : IRequestHandler<AdminGetAllGroupInvitationsByGroupIdQuery, AdminGetAllGroupInvitationsByGroupIdResponse>
{
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly IGroupRepository _groupRepository;

    public AdminGetAllGroupInvitationsByGroupIdHandler(
        IGroupInvitationRepository groupInvitationRepository,
        IGroupRepository groupRepository)
    {
        _groupInvitationRepository = groupInvitationRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetAllGroupInvitationsByGroupIdResponse> Handle(AdminGetAllGroupInvitationsByGroupIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (invitations, hits) = 
            await _groupInvitationRepository.GetByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

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
