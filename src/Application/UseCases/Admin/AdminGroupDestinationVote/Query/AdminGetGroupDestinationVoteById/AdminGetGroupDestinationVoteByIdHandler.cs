using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

public class AdminGetGroupDestinationVoteByIdHandler : IRequestHandler<AdminGetGroupDestinationVoteByIdQuery, AdminGetGroupDestinationVoteByIdResponse>
{
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;
    private readonly IGroupRepository _groupRepository;

    public AdminGetGroupDestinationVoteByIdHandler(
        IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
        IGroupRepository groupRepository)
    {
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetGroupDestinationVoteByIdResponse> Handle(AdminGetGroupDestinationVoteByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var vote = await _groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(request.GroupId, 
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
