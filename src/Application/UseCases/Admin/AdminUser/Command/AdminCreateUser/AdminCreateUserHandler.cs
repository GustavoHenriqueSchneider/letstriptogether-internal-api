using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminCreateUser;

public class AdminCreateUserHandler : IRequestHandler<AdminCreateUserCommand, AdminCreateUserResponse>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public AdminCreateUserHandler(
        IPasswordHashService passwordHashService,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _passwordHashService = passwordHashService;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<AdminCreateUserResponse> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var existsUserWithEmail = await _userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {

            throw new ConflictException("There is already an user using this email.");
        }
        var defaultRole = await _roleRepository.GetDefaultUserRoleAsync(cancellationToken);
        if (defaultRole is null)
        {
            throw new NotFoundException("Role not found.");
        }
        var passwordHash = _passwordHashService.HashPassword(request.Password);
        var user = new UserModel(request.Name, email, passwordHash, defaultRole);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new AdminCreateUserResponse { Id = user.Id };
    }
}
