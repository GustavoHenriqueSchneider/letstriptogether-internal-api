using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;

public class AdminCreateUserHandler(
    IPasswordHashService passwordHashService,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<AdminCreateUserCommand, AdminCreateUserResponse>
{
    public async Task<AdminCreateUserResponse> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {

            throw new ConflictException("There is already an user using this email.");
        }
        var defaultRole = await roleRepository.GetDefaultUserRoleAsync(cancellationToken);
        if (defaultRole is null)
        {
            throw new NotFoundException("Role not found.");
        }
        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new UserModel(request.Name, email, passwordHash, defaultRole);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new AdminCreateUserResponse { Id = user.Id };
    }
}
