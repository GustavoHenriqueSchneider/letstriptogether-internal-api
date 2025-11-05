using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdHandler : IRequestHandler<AdminAnonymizeUserByIdCommand>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IRedisService _redisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserGroupInvitationRepository _userGroupInvitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public AdminAnonymizeUserByIdHandler(
        IGroupMemberRepository groupMemberRepository,
        IRedisService redisService,
        IUnitOfWork unitOfWork,
        IUserGroupInvitationRepository userGroupInvitationRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _redisService = redisService;
        _unitOfWork = unitOfWork;
        _userGroupInvitationRepository = userGroupInvitationRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task Handle(AdminAnonymizeUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithRelationshipsByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        _groupMemberRepository.RemoveRange(user.GroupMemberships);
        _userGroupInvitationRepository.RemoveRange(user.AcceptedInvitations);
        _userRoleRepository.RemoveRange(user.UserRoles);

        user.Anonymize();

        _userRepository.Update(user);
        await _unitOfWork.SaveAsync(cancellationToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        await _redisService.DeleteAsync(key);

        // TODO: registrar log de auditoria da anonimiza??o do usu?rio
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos
    }
}
