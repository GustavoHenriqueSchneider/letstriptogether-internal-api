using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;

public class GetActiveGroupInvitationHandler : IRequestHandler<GetActiveGroupInvitationQuery, GetActiveGroupInvitationResponse>
{
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public GetActiveGroupInvitationHandler(
        IGroupInvitationRepository groupInvitationRepository,
        IGroupRepository groupRepository,
        ITokenService tokenService,
        IUserRepository userRepository)
    {
        _groupInvitationRepository = groupInvitationRepository;
        _groupRepository = groupRepository;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<GetActiveGroupInvitationResponse> Handle(GetActiveGroupInvitationQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        if (!currentUserMember.IsOwner)
        {
            throw new BadRequestException("Only the group owner can get group invitation.");
        }

        var activeInvitation = 
            await _groupInvitationRepository.GetByGroupAndStatusAsync(request.GroupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (activeInvitation is null)
        {
            throw new NotFoundException("Active invitation not found.");
        }
        
        var invitationToken = _tokenService.GenerateInvitationToken(activeInvitation.Id);
        return new GetActiveGroupInvitationResponse { Token = invitationToken };
    }
}
