using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;

[TestFixture]
public class CancelActiveGroupInvitationHandlerTests : TestBase
{
    private CancelActiveGroupInvitationHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new CancelActiveGroupInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _unitOfWork,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithOwner_ShouldCancelInvitation()
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

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new CancelActiveGroupInvitationCommand
        {
            UserId = user.Id,
            GroupId = group.Id
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var cancelledInvitation = await _groupInvitationRepository.GetByIdAsync(invitation.Id, CancellationToken.None);
        cancelledInvitation.Should().NotBeNull();
        cancelledInvitation!.Status.Should().Be(GroupInvitationStatus.Cancelled);
    }
}
