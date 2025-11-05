using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;

public class AcceptInvitationHandler : IRequestHandler<AcceptInvitationCommand>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserGroupInvitationRepository _userGroupInvitationRepository;
    private readonly IUserRepository _userRepository;

    public AcceptInvitationHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository,
        IGroupInvitationRepository groupInvitationRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IUserGroupInvitationRepository userGroupInvitationRepository,
        IUserRepository userRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
        _groupInvitationRepository = groupInvitationRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _userGroupInvitationRepository = userGroupInvitationRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await _userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);
        
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        if (user.Preferences is null)
        {
            throw new BadRequestException("User has not filled any preferences yet.");
        }
        
        _ = new UserPreference(user.Preferences);

        var (isValid, id) = _tokenService.ValidateInvitationToken(request.Token);

        if (!isValid)
        {
            throw new UnauthorizedException("Invalid invitation token.");
        }

        var (isExpired, _) = _tokenService.IsTokenExpired(request.Token);

        if (isExpired)
        {
            throw new UnauthorizedException("Invitation token has expired.");
        }

        if (!Guid.TryParse(id, out var invitationId) || invitationId == Guid.Empty)
        {
            throw new NotFoundException("Invitation not found.");
        }
        
        var groupInvitation = await _groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId, cancellationToken);
        if (groupInvitation is null)
        {
            throw new NotFoundException("Invitation not found.");
        }
        
        if (groupInvitation.Status != GroupInvitationStatus.Active)
        {
            throw new BadRequestException("Invitation is not active.");
        }

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            groupInvitation.Expire();
            
            _groupInvitationRepository.Update(groupInvitation);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            throw new BadRequestException("Invitation is not active.");
        }

        var existingAnswer = groupInvitation.AnsweredBy.Any(x => x.UserId == currentUserId);
        if (existingAnswer)
        {
            throw new ConflictException("You have already answered this invitation.");
        }

        var group = await _groupRepository.GetGroupWithMembersAndMatchesAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isAlreadyMember = group.Members.Any(x => x.UserId == currentUserId);
        if (isAlreadyMember)
        {
            throw new BadRequestException("You are already a member of this group.");
        }
        
        var invitationAnswer = groupInvitation.AddAnswer(currentUserId, isAccepted: true);
        var groupMember = group.AddMember(user, false);
        
        await _userGroupInvitationRepository.AddAsync(invitationAnswer, cancellationToken);
        await _groupMemberRepository.AddAsync(groupMember, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        var groupToUpdate = await _groupRepository.GetGroupWithMembersPreferencesAsync(group.Id, cancellationToken);
        if (groupToUpdate is null)
        {
            throw new NotFoundException("Group not found.");
        }
        
        groupToUpdate.UpdatePreferences();
        
        _groupRepository.Update(groupToUpdate);
        _groupPreferenceRepository.Update(groupToUpdate.Preferences);

        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
