using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMatch.Query.AdminGetGroupMatchById;

public class AdminGetGroupMatchByIdHandler : IRequestHandler<AdminGetGroupMatchByIdQuery, AdminGetGroupMatchByIdResponse>
{
    private readonly IGroupRepository _groupRepository;

    public AdminGetGroupMatchByIdHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetGroupMatchByIdResponse> Handle(AdminGetGroupMatchByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupWithMatchesAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var groupMatch = group.Matches.SingleOrDefault(x => x.Id == request.MatchId);

        if (groupMatch is null)
        {
            throw new NotFoundException("Group match not found.");
        }

        return new AdminGetGroupMatchByIdResponse
        {
            DestinationId = groupMatch.DestinationId,
            CreatedAt = groupMatch.CreatedAt,
            UpdatedAt = groupMatch.UpdatedAt
        };
    }
}
