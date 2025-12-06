using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.RoleAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.v1.Auth.Command.Register;

public class RegisterHandler(
    IPasswordHashService passwordHashService,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (!request.HasAcceptedTermsOfUse)
        {
            throw new BadRequestException("Terms of use must be accepted for user registration.");
        }

        var email = request.Email.ToLowerInvariant();
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {
            throw new ConflictException("There is already an user using this email.");
        }

        var defaultRole = await roleRepository.GetDefaultUserRoleAsync(cancellationToken);

        if (defaultRole is null)
        {
            throw new NotFoundException("Default role not found.");
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new UserModel(request.Name, email, passwordHash, defaultRole);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new RegisterResponse { Id = user.Id };
    }
}
