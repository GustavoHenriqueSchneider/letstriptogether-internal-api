using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using GroupInvitationModel = LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;

public class CreateGroupInvitationHandler : IRequestHandler<CreateGroupInvitationCommand, CreateGroupInvitationResponse>
{
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CreateGroupInvitationHandler(
        IGroupInvitationRepository groupInvitationRepository,
        IGroupRepository groupRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupInvitationRepository = groupInvitationRepository;
        _groupRepository = groupRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<CreateGroupInvitationResponse> Handle(CreateGroupInvitationCommand request, CancellationToken cancellationToken)
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
            throw new BadRequestException("Only the group owner can create invitations.");
        }

        GroupInvitationModel groupInvitation;
        
        var existingInvitation = 
            await _groupInvitationRepository.GetByGroupAndStatusAsync(request.GroupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (existingInvitation is not null)
        {
            groupInvitation = existingInvitation.Copy();
            _groupInvitationRepository.Update(existingInvitation);
        }
        else
        {
            groupInvitation = new GroupInvitationModel(request.GroupId);
        }

        await _groupInvitationRepository.AddAsync(groupInvitation, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        var invitationToken = _tokenService.GenerateInvitationToken(groupInvitation.Id);

        return new CreateGroupInvitationResponse { Token = invitationToken };
    }
}
