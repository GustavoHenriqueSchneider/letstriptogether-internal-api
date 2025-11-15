using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using MediatR;

namespace Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdHandler(IGroupRepository groupRepository)
    : IRequestHandler<AdminGetGroupMemberByIdQuery, AdminGetGroupMemberByIdResponse>
{
    public async Task<AdminGetGroupMemberByIdResponse> Handle(AdminGetGroupMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var groupMember = group.Members.SingleOrDefault(x => x.Id == request.MemberId);

        if (groupMember is null)
        {
            throw new NotFoundException("Group member not found.");
        }

        return new AdminGetGroupMemberByIdResponse
        {
            UserId = groupMember.UserId,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        };
    }
}
