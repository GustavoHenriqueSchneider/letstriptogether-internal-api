using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdHandler : IRequestHandler<AdminGetGroupMemberByIdQuery, AdminGetGroupMemberByIdResponse>
{
    private readonly IGroupRepository _groupRepository;

    public AdminGetGroupMemberByIdHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetGroupMemberByIdResponse> Handle(AdminGetGroupMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

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
