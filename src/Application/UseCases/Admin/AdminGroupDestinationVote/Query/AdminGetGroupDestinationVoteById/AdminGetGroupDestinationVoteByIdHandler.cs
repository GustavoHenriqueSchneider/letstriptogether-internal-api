using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

public class AdminGetGroupDestinationVoteByIdHandler(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<AdminGetGroupDestinationVoteByIdQuery, AdminGetGroupDestinationVoteByIdResponse>
{
    public async Task<AdminGetGroupDestinationVoteByIdResponse> Handle(AdminGetGroupDestinationVoteByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(request.GroupId, 
            request.DestinationVoteId, cancellationToken);

        if (vote is null)
        {
            throw new NotFoundException("Group member destination vote not found.");
        }

        return new AdminGetGroupDestinationVoteByIdResponse
        {
            MemberId = vote.GroupMemberId,
            DestinationId = vote.DestinationId,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt,
            UpdatedAt = vote.UpdatedAt
        };
    }
}
