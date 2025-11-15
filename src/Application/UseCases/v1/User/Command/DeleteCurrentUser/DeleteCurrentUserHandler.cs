using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.v1.User.Command.DeleteCurrentUser;

public class DeleteCurrentUserHandler(
    IRedisService redisService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<DeleteCurrentUserCommand>
{
    public async Task Handle(DeleteCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        userRepository.Remove(user);
        await unitOfWork.SaveAsync(cancellationToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        await redisService.DeleteAsync(key);
    }
}
