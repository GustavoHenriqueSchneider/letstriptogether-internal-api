using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;

public class RefuseInvitationHandler : IRequestHandler<RefuseInvitationCommand>
{
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserGroupInvitationRepository _userGroupInvitationRepository;
    private readonly IUserRepository _userRepository;

    public RefuseInvitationHandler(
        IGroupInvitationRepository groupInvitationRepository,
        IGroupRepository groupRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IUserGroupInvitationRepository userGroupInvitationRepository,
        IUserRepository userRepository)
    {
        _groupInvitationRepository = groupInvitationRepository;
        _groupRepository = groupRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _userGroupInvitationRepository = userGroupInvitationRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(RefuseInvitationCommand request, CancellationToken cancellationToken)
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

        var group = await _groupRepository.GetGroupWithMembersAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isAlreadyMember = group.Members.Any(x => x.UserId == currentUserId);
        if (isAlreadyMember)
        {
            throw new BadRequestException("You are already a member of this group.");
        }
        
        var invitationAnswer = groupInvitation.AddAnswer(currentUserId, isAccepted: false);
        
        await _userGroupInvitationRepository.AddAsync(invitationAnswer, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
