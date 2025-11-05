using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.AnonymizeCurrentUser;

public class AnonymizeCurrentUserHandler(
    IGroupMemberRepository groupMemberRepository,
    IRedisService redisService,
    IUnitOfWork unitOfWork,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository)
    : IRequestHandler<AnonymizeCurrentUserCommand>
{
    public async Task Handle(AnonymizeCurrentUserCommand request, CancellationToken cancellationToken)
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
