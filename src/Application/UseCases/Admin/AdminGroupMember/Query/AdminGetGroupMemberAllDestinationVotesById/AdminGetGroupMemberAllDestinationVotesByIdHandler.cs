using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

public class AdminGetGroupMemberAllDestinationVotesByIdHandler : IRequestHandler<AdminGetGroupMemberAllDestinationVotesByIdQuery, AdminGetGroupMemberAllDestinationVotesByIdResponse>
{
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;
    private readonly IGroupRepository _groupRepository;

    public AdminGetGroupMemberAllDestinationVotesByIdHandler(
        IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
        IGroupRepository groupRepository)
    {
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetGroupMemberAllDestinationVotesByIdResponse> Handle(AdminGetGroupMemberAllDestinationVotesByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isGroupMember = group.Members.Any(m => m.Id == request.MemberId);

        if (!isGroupMember)
        {
            throw new NotFoundException("The user is not a member of this group.");
        }

        var (votes, hits) = await _groupMemberDestinationVoteRepository.GetByMemberIdAsync(request.MemberId,
            request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetGroupMemberAllDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new AdminGetGroupMemberAllDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
