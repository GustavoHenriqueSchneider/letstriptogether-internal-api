using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;

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

        var email = request.Email;
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
