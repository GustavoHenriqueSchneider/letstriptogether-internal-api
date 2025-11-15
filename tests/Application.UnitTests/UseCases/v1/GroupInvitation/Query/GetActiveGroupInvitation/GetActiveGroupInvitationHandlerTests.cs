using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.GroupInvitation.Query.GetActiveGroupInvitation;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.GroupInvitation.Query.GetActiveGroupInvitation;

[TestFixture]
public class GetActiveGroupInvitationHandlerTests : TestBase
{
    private GetActiveGroupInvitationHandler _handler = null!;
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private GroupRepository _groupRepository = null!;
    private ITokenService _tokenService = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.GenerateInvitationToken(It.IsAny<Guid>()))
            .Returns("test-token");
        _tokenService = tokenServiceMock.Object;
        
        _handler = new GetActiveGroupInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithActiveInvitation_ShouldReturnToken()
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

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetActiveGroupInvitationQuery
        {
            UserId = user.Id,
            GroupId = group.Id
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
    }

    [Test]
    public async Task Handle_WithNoActiveInvitation_ShouldThrowNotFoundException()
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

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetActiveGroupInvitationQuery
        {
            UserId = user.Id,
            GroupId = group.Id
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
