using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.UpdateCurrentUser;

public class UpdateCurrentUserHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository) : IRequestHandler<UpdateCurrentUserCommand>
{
    public async Task Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        user.Update(request.Name);

        userRepository.Update(user);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
