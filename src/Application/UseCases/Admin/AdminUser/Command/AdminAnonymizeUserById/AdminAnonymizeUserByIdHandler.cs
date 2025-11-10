using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdHandler(
    IGroupMemberRepository groupMemberRepository,
    IRedisService redisService,
    IUnitOfWork unitOfWork,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository)
    : IRequestHandler<AdminAnonymizeUserByIdCommand>
{
    public async Task Handle(AdminAnonymizeUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRelationshipsByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        groupMemberRepository.RemoveRange(user.GroupMemberships);
        userGroupInvitationRepository.RemoveRange(user.AcceptedInvitations);
        userRoleRepository.RemoveRange(user.UserRoles);

        user.Anonymize();

        userRepository.Update(user);
        await unitOfWork.SaveAsync(cancellationToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        await redisService.DeleteAsync(key);
    }
}
