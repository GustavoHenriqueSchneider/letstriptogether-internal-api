using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
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
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
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
    public void Handle_WithInvalidUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AdminUpdateUserByIdCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            Name = TestDataHelper.GenerateRandomName()
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}
