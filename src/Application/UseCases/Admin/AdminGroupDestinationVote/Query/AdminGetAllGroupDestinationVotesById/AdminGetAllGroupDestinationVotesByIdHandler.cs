using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

public class AdminGetAllGroupDestinationVotesByIdHandler : IRequestHandler<AdminGetAllGroupDestinationVotesByIdQuery, AdminGetAllGroupDestinationVotesByIdResponse>
{
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;

    public AdminGetAllGroupDestinationVotesByIdHandler(IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository)
    {
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
    }

    public async Task<AdminGetAllGroupDestinationVotesByIdResponse> Handle(AdminGetAllGroupDestinationVotesByIdQuery request, CancellationToken cancellationToken)
    {
        var (votes, hits) = 
            await _groupMemberDestinationVoteRepository.GetByGroupIdAsync(request.GroupId, 
                request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllGroupDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new AdminGetAllGroupDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
