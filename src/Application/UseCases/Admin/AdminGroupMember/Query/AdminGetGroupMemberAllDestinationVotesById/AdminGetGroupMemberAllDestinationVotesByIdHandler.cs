using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using MediatR;

namespace Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

public class AdminGetGroupMemberAllDestinationVotesByIdHandler(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<AdminGetGroupMemberAllDestinationVotesByIdQuery,
        AdminGetGroupMemberAllDestinationVotesByIdResponse>
{
    public async Task<AdminGetGroupMemberAllDestinationVotesByIdResponse> Handle(AdminGetGroupMemberAllDestinationVotesByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isGroupMember = group.Members.Any(m => m.Id == request.MemberId);

        if (!isGroupMember)
        {
            throw new NotFoundException("The user is not a member of this group.");
        }

        var (votes, hits) = await groupMemberDestinationVoteRepository.GetByMemberIdAsync(request.MemberId,
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
