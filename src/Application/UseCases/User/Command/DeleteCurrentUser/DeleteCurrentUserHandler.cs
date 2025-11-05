using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.DeleteCurrentUser;

public class DeleteCurrentUserHandler : IRequestHandler<DeleteCurrentUserCommand>
{
    private readonly IRedisService _redisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public DeleteCurrentUserHandler(
        IRedisService redisService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _redisService = redisService;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(DeleteCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        // TODO: parou de funcionar
        _userRepository.Remove(user);
        await _unitOfWork.SaveAsync(cancellationToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        await _redisService.DeleteAsync(key);
    }
}
