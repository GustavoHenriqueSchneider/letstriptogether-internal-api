using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;

public class GetAllGroupMatchesByIdHandler : IRequestHandler<GetAllGroupMatchesByIdQuery, GetAllGroupMatchesByIdResponse>
{
    private readonly IGroupMatchRepository _groupMatchRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetAllGroupMatchesByIdHandler(
        IGroupMatchRepository groupMatchRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupMatchRepository = groupMatchRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetAllGroupMatchesByIdResponse> Handle(GetAllGroupMatchesByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var existsUser = await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken);

        if (!existsUser)
        {
            throw new NotFoundException("User not found.");
        }

        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var isGroupMember = await _groupRepository.IsGroupMemberByUserIdAsync(request.GroupId, currentUserId, cancellationToken);

        if (!isGroupMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var (groupMatches, hits) =
            await _groupMatchRepository.GetByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        return new GetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new GetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
