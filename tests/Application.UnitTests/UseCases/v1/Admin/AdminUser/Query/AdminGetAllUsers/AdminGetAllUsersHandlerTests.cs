using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Admin.AdminUser.Query.AdminGetAllUsers;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminUser.Query.AdminGetAllUsers;

[TestFixture]
public class AdminGetAllUsersHandlerTests : TestBase
{
    private AdminGetAllUsersHandler _handler = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new AdminGetAllUsersHandler(_userRepository);
    }

    [Test]
    public async Task Handle_WithUsers_ShouldReturnPaginatedResults()
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

        for (int i = 0; i < 15; i++)
        {
            var email = TestDataHelper.GenerateRandomEmail();
            var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
            var userName = TestDataHelper.GenerateRandomName();
            var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
            await _userRepository.AddAsync(user, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        var query = new AdminGetAllUsersQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Hits.Should().Be(15);
    }
}
