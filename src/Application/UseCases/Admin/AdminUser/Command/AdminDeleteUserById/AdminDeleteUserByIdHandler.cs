using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdHandler(
    IRedisService redisService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<AdminDeleteUserByIdCommand>
{
    public async Task Handle(AdminDeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserRefreshToken(user.Id);
        await redisService.DeleteAsync(key);
        
        userRepository.Remove(user);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
