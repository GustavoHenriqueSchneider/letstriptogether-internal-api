using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminCreateUser;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminCreateUser;

[TestFixture]
public class AdminCreateUserHandlerTests : TestBase
{
    private AdminCreateUserHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IPasswordHashService _passwordHashService = null!;
    private RoleRepository _roleRepository = null!;
    private UserRepository _userRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _roleRepository = new RoleRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        
        _handler = new AdminCreateUserHandler(
            _passwordHashService,
            _roleRepository,
            _unitOfWork,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidData_ShouldCreateUser()
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

        var command = new AdminCreateUserCommand
        {
            Name = TestDataHelper.GenerateRandomName(),
            Email = TestDataHelper.GenerateRandomEmail(),
            Password = TestDataHelper.GenerateValidPassword()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        
        var createdUser = await _userRepository.GetByEmailAsync(command.Email, CancellationToken.None);
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be(command.Email);
    }

    [Test]
    public async Task Handle_WithDuplicateEmail_ShouldThrowConflictException()
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
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new AdminCreateUserCommand
        {
            Name = TestDataHelper.GenerateRandomName(),
            Email = email,
            Password = TestDataHelper.GenerateValidPassword()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ConflictException>();
    }
}
