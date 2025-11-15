using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.RoleAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminCreateUser;

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
