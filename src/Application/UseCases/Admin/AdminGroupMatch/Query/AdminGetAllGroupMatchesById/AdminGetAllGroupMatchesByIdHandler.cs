using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

public class AdminGetAllGroupMatchesByIdHandler : IRequestHandler<AdminGetAllGroupMatchesByIdQuery, AdminGetAllGroupMatchesByIdResponse>
{
    private readonly IGroupMatchRepository _groupMatchRepository;
    private readonly IGroupRepository _groupRepository;

    public AdminGetAllGroupMatchesByIdHandler(
        IGroupMatchRepository groupMatchRepository,
        IGroupRepository groupRepository)
    {
        _groupMatchRepository = groupMatchRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetAllGroupMatchesByIdResponse> Handle(AdminGetAllGroupMatchesByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (groupMatches, hits) = 
            await _groupMatchRepository.GetByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

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
