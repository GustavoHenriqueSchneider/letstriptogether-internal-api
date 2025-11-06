using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories.Users;

[TestFixture]
public class UserGroupInvitationRepositoryTests : TestBase
{
    private UserGroupInvitationRepository _repository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private GroupRepository _groupRepository = null!;
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new UserGroupInvitationRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
    }

    [Test]
    public async Task ExistsByUserIdAndGroupInvitationIdAsync_WithExisting_ShouldReturnTrue()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = $"Test Group {Guid.NewGuid():N}";
        var group = new Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userGroupInvitation = new UserGroupInvitation
        {
            UserId = user.Id,
            GroupInvitationId = invitation.Id,
            IsAccepted = true
        };
        typeof(UserGroupInvitation).GetProperty("UserId")!.SetValue(userGroupInvitation, user.Id);
        typeof(UserGroupInvitation).GetProperty("GroupInvitationId")!.SetValue(userGroupInvitation, invitation.Id);
        typeof(UserGroupInvitation).GetProperty("IsAccepted")!.SetValue(userGroupInvitation, true);
        await _repository.AddAsync(userGroupInvitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByUserIdAndGroupInvitationIdAsync(user.Id, invitation.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsByUserIdAndGroupInvitationIdAsync_WithNonExisting_ShouldReturnFalse()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = $"Test Group {Guid.NewGuid():N}";
        var group = new Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByUserIdAndGroupInvitationIdAsync(user.Id, invitation.Id, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
