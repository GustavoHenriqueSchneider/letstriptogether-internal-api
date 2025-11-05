using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

public class AdminUpdateUserByIdHandler : IRequestHandler<AdminUpdateUserByIdCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public AdminUpdateUserByIdHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(AdminUpdateUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        user.Update(request.Name);

        _userRepository.Update(user);
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
