using Domain.Aggregates.RoleAggregate.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Tests.Common;
using NUnit.Framework;
using RoleType = Domain.Security.Roles;

namespace Infrastructure.Tests.Repositories.Roles;

[TestFixture]
public class RoleRepositoryTests : TestBase
{
    private RoleRepository _repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        _repository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task GetDefaultUserRoleAsync_WithDefaultRole_ShouldReturnRole()
    {
        // Arrange
        var role = await _repository.GetByNameAsync(RoleType.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, RoleType.User);
            await _repository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        // Act
        var result = await _repository.GetDefaultUserRoleAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(RoleType.User);
    }
}
