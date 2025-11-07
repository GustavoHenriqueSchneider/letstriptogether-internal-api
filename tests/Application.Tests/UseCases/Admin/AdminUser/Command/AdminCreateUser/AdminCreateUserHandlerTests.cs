using Application.Tests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Command.AdminCreateUser;

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
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
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
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.ConflictException>();
    }
}
