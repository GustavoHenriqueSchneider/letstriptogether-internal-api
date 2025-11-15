using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminUpdateUserById;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

[TestFixture]
public class AdminUpdateUserByIdHandlerTests : TestBase
{
    private AdminUpdateUserByIdHandler _handler = null!;
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
        
        _handler = new AdminUpdateUserByIdHandler(_unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidUser_ShouldUpdateUser()
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

        var newName = TestDataHelper.GenerateRandomName();
        var command = new AdminUpdateUserByIdCommand
        {
            UserId = user.Id,
            Name = newName
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be(newName);
    }

    [Test]
    public async Task Handle_WithInvalidUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AdminUpdateUserByIdCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            Name = TestDataHelper.GenerateRandomName()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
