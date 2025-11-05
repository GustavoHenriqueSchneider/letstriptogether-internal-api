using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

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

        // TODO: parou de funcionar
        userRepository.Remove(user);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
