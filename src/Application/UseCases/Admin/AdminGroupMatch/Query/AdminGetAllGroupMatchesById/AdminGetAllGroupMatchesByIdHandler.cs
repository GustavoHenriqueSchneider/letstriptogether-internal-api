using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

public class AdminGetAllGroupMatchesByIdHandler(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<AdminGetAllGroupMatchesByIdQuery, AdminGetAllGroupMatchesByIdResponse>
{
    public async Task<AdminGetAllGroupMatchesByIdResponse> Handle(AdminGetAllGroupMatchesByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (groupMatches, hits) = 
            await groupMatchRepository.GetByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new AdminGetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
