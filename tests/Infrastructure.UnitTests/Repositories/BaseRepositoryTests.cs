using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class BaseRepositoryTests : TestBase
{
    private BaseRepository<Role> _repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        _repository = new BaseRepository<Role>(DbContext);
    }

    [Test]
    public async Task AddAsync_WithEntity_ShouldAddToDatabase()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, $"Test{Guid.NewGuid().ToString()[..10]}");

        // Act
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        var exists = await _repository.ExistsByIdAsync(role.Id, CancellationToken.None);
        exists.Should().BeTrue();
    }

    [Test]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
    {
        // Arrange
        var role = new Role { Name = $"Role{Guid.NewGuid().ToString()[..8]}" };
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(role.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(role.Name);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ExistsByIdAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var role = new Role { Name = $"Role{Guid.NewGuid().ToString()[..8]}" };
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByIdAsync(role.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsByIdAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsByIdAsync(nonExistingId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task GetAllAsync_WithEntities_ShouldReturnPaginatedResults()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            var role = new Role { Name = $"Role{Guid.NewGuid().ToString()[..11]}" };
            await _repository.AddAsync(role, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        // Act
        var (data, hits) = await _repository.GetAllAsync(1, 10, CancellationToken.None);

        // Assert
        data.Should().HaveCount(10);
        hits.Should().BeGreaterOrEqualTo(15);
    }

    [Test]
    public async Task Update_WithEntity_ShouldUpdateInDatabase()
    {
        // Arrange
        var role = new Role { Name = $"up{Guid.NewGuid().ToString()[..11]}" };
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        role.GetType().GetProperty("Name")!.SetValue(role, $"2up{Guid.NewGuid().ToString()[..9]}");
        _repository.Update(role);
        await DbContext.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(role.Id, CancellationToken.None);
        updated.Should().NotBeNull();
    }

    [Test]
    public async Task Update_WithDetachedEntity_ShouldAttachAndUpdate()
    {
        // Arrange
        var role = new Role { Name = $"det{Guid.NewGuid().ToString()[..8]}" };
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Detach entity
        DbContext.Entry(role).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Modify entity
        var newName = $"upd{Guid.NewGuid().ToString()[..8]}";
        role.GetType().GetProperty("Name")!.SetValue(role, newName);

        // Act
        _repository.Update(role);
        await DbContext.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(role.Id, CancellationToken.None);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(newName);
    }

    [Test]
    public async Task Update_WithDetachedEntityFoundInContext_ShouldUpdateTrackedEntity()
    {
        // Arrange
        var role = new Role { Name = $"trac{Guid.NewGuid().ToString()[..8]}" };
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Create a new instance with same ID (simulating detached entity)
        var detachedRole = new Role();
        typeof(Role).GetProperty("Id")!.SetValue(detachedRole, role.Id);
        typeof(Role).GetProperty("Name")!.SetValue(detachedRole, $"newName{Guid.NewGuid().ToString()[..8]}");

        // Act
        _repository.Update(detachedRole);
        await DbContext.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(role.Id, CancellationToken.None);
        updated.Should().NotBeNull();
    }

    [Test]
    public async Task Update_WithTrackedEntity_ShouldUpdateState()
    {
        // Arrange
        var role = new Role { Name = $"tracked{Guid.NewGuid().ToString()[..8]}" };
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Entity is already tracked
        var newName = $"mod{Guid.NewGuid().ToString()[..8]}";
        role.GetType().GetProperty("Name")!.SetValue(role, newName);

        // Act
        _repository.Update(role);
        await DbContext.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(role.Id, CancellationToken.None);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(newName);
    }

    [Test]
    public async Task Remove_WithEntity_ShouldRemoveFromDatabase()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, $"new{Guid.NewGuid().ToString()[..10]}");
        await _repository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        _repository.Remove(role);
        await DbContext.SaveChangesAsync();

        // Assert
        var exists = await _repository.ExistsByIdAsync(role.Id, CancellationToken.None);
        exists.Should().BeFalse();
    }

    [Test]
    public async Task AddRangeAsync_WithEntities_ShouldAddAllToDatabase()
    {
        // Arrange
        var roles = new List<Role>();
        for (int i = 0; i < 5; i++)
        {
            var role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, $"Test{i}{Guid.NewGuid().ToString()[..8]}");
            roles.Add(role);
        }

        // Act
        await _repository.AddRangeAsync(roles, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        foreach (var role in roles)
        {
            var exists = await _repository.ExistsByIdAsync(role.Id, CancellationToken.None);
            exists.Should().BeTrue();
        }
    }

    [Test]
    public async Task GetHitsAsync_WithEntities_ShouldReturnCount()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            var role = new Role { Name = $"Role{Guid.NewGuid().ToString()[..8]}" };
            await _repository.AddAsync(role, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        // Act
        var hits = await _repository.GetHitsAsync(CancellationToken.None);

        // Assert
        hits.Should().BeGreaterOrEqualTo(5);
    }
}
