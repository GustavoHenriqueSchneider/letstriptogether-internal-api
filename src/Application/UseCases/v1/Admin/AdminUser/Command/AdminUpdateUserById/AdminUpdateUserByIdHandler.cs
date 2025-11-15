using Application.Common.Exceptions;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminUpdateUserById;

public class AdminUpdateUserByIdHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository) : IRequestHandler<AdminUpdateUserByIdCommand>
{
    public async Task Handle(AdminUpdateUserByIdCommand request, CancellationToken cancellationToken)
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
