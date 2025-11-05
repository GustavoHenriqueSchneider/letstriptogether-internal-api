using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdHandler : IRequestHandler<AdminDeleteUserByIdCommand>
{
    private readonly IRedisService _redisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public AdminDeleteUserByIdHandler(
        IRedisService redisService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _redisService = redisService;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(AdminDeleteUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserRefreshToken(user.Id);
        await _redisService.DeleteAsync(key);

        // TODO: parou de funcionar
        _userRepository.Remove(user);
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
