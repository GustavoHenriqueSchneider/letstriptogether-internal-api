using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Invitation.Query.GetInvitationDetails;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Invitation.Query.GetInvitationDetails;

[TestFixture]
public class GetInvitationDetailsHandlerTests : TestBase
{
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private GroupRepository _groupRepository = null!;
    private RoleRepository _roleRepository = null!;
    private UserRepository _userRepository = null!;
    private ITokenService _tokenService = null!;
    private PasswordHashService _passwordHashService = null!;
    private GetInvitationDetailsHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();

        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, Guid.NewGuid().ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);
    }

    [Test]
    public async Task Handle_WithValidToken_ShouldReturnInvitationDetails()
    {
        // Arrange
        var role = await EnsureUserRoleAsync();
        var owner = await CreateUserAsync(role);
        await DbContext.SaveChangesAsync();

        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(
            TestDataHelper.GenerateRandomGroupName(),
            DateTime.UtcNow.AddDays(10));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);

        var query = new GetInvitationDetailsQuery
        {
            Token = "valid-token"
        };

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.CreatedBy.Should().Be(owner.Name);
        response.GroupName.Should().Be(group.Name);
        response.IsActive.Should().BeTrue();
    }

    [Test]
    public async Task Handle_WithExpiredInvitation_ShouldReturnInactiveStatus()
    {
        // Arrange
        var role = await EnsureUserRoleAsync();
        var owner = await CreateUserAsync(role);
        await DbContext.SaveChangesAsync();

        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(
            TestDataHelper.GenerateRandomGroupName(),
            DateTime.UtcNow.AddDays(10));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        var expirationField = typeof(Domain.Aggregates.GroupAggregate.Entities.GroupInvitation)
            .GetField("_expirationDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        expirationField!.SetValue(invitation, DateTime.UtcNow.AddDays(-1));

        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);

        var query = new GetInvitationDetailsQuery
        {
            Token = "expired-invitation-token"
        };

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsActive.Should().BeFalse();
        response.GroupName.Should().Be(group.Name);
    }

    [Test]
    public void Handle_WithInvalidToken_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((false, null));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);

        var query = new GetInvitationDetailsQuery
        {
            Token = "invalid-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid invitation token.");
    }

    [Test]
    public void Handle_WithExpiredToken_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, Guid.NewGuid().ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((true, DateTime.UtcNow.AddDays(-1)));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);

        var query = new GetInvitationDetailsQuery
        {
            Token = "expired-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invitation token has expired.");
    }

    [Test]
    public void Handle_WithInvalidGuidInToken_ShouldThrowNotFoundException()
    {
        // Arrange
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, "invalid-guid"));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);

        var query = new GetInvitationDetailsQuery
        {
            Token = "token-with-invalid-guid"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Invitation not found.");
    }

    [Test]
    public async Task Handle_WithNonExistentInvitation_ShouldThrowNotFoundException()
    {
        // Arrange
        var nonExistentInvitationId = Guid.NewGuid();
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, nonExistentInvitationId.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;

        _handler = new GetInvitationDetailsHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork);

        var query = new GetInvitationDetailsQuery
        {
            Token = "token-for-non-existent-invitation"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Invitation not found.");
    }

    private async Task<Role> EnsureUserRoleAsync()
    {
        var role = await _roleRepository.GetByNameAsync(Domain.Security.Roles.User, CancellationToken.None);
        if (role is not null)
        {
            return role;
        }

        role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Domain.Security.Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();
        return role;
    }

    private async Task<Domain.Aggregates.UserAggregate.Entities.User> CreateUserAsync(Role role)
    {
        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var name = TestDataHelper.GenerateRandomName();

        var user = new Domain.Aggregates.UserAggregate.Entities.User(name, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        return user;
    }
}

