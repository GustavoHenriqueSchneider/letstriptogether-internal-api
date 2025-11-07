using Application.Tests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.UpdateCurrentUser;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.Tests.UseCases.User.Command.UpdateCurrentUser;

[TestFixture]
public class UpdateCurrentUserHandlerTests : TestBase
{
    private UpdateCurrentUserHandler _handler = null!;
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
        
        _handler = new UpdateCurrentUserHandler(_unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidData_ShouldUpdateUser()
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
        var originalName = "Original Name";
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(originalName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var newName = TestDataHelper.GenerateRandomName();
        var command = new UpdateCurrentUserCommand
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
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateCurrentUserCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            Name = TestDataHelper.GenerateRandomName()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>();
    }
}
