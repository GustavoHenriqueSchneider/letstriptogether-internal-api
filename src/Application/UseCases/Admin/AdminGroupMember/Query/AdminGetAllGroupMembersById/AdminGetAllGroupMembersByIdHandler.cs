using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;

public class AdminGetAllGroupMembersByIdHandler : IRequestHandler<AdminGetAllGroupMembersByIdQuery, AdminGetAllGroupMembersByIdResponse>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupRepository _groupRepository;

    public AdminGetAllGroupMembersByIdHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupRepository groupRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetAllGroupMembersByIdResponse> Handle(AdminGetAllGroupMembersByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (groupMembers, hits) =
            await _groupMemberRepository.GetAllByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllGroupMembersByIdResponse
        {
            Data = groupMembers.Select(x => new AdminGetAllGroupMembersByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
