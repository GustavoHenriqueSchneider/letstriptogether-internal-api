using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using NUnit.Framework;
using RoleType = LetsTripTogether.InternalApi.Domain.Security.Roles;

namespace Infrastructure.UnitTests.Repositories.Roles;

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
        var role = await _repository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
