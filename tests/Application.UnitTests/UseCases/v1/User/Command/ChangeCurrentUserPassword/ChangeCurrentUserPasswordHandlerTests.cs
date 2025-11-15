using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.User.Command.ChangeCurrentUserPassword;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.User.Command.ChangeCurrentUserPassword;

[TestFixture]
public class ChangeCurrentUserPasswordHandlerTests : TestBase
{
    private ChangeCurrentUserPasswordHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new ChangeCurrentUserPasswordHandler(_passwordHashService, _unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidData_ShouldChangePassword()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var currentPassword = TestDataHelper.GenerateValidPassword();
        var currentPasswordHash = _passwordHashService.HashPassword(currentPassword);
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, currentPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var newPassword = TestDataHelper.GenerateValidPassword();
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = user.Id,
            CurrentPassword = currentPassword,
            NewPassword = newPassword
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        var isNewPasswordValid = _passwordHashService.VerifyPassword(newPassword, updatedUser!.PasswordHash);
        isNewPasswordValid.Should().BeTrue();
    }

    [Test]
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            CurrentPassword = TestDataHelper.GenerateValidPassword(),
            NewPassword = TestDataHelper.GenerateValidPassword()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Test]
    public async Task Handle_WithIncorrectCurrentPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var currentPassword = TestDataHelper.GenerateValidPassword();
        var currentPasswordHash = _passwordHashService.HashPassword(currentPassword);
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, currentPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var wrongPassword = "WrongPassword123!";
        var newPassword = TestDataHelper.GenerateValidPassword();
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = user.Id,
            CurrentPassword = wrongPassword,
            NewPassword = newPassword
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid current password.");
    }
}
