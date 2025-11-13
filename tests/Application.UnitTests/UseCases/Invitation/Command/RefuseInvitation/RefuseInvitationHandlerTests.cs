using System.IdentityModel.Tokens.Jwt;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.Invitation.Command.RefuseInvitation;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;
using Domain.Security;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Invitation.Command.RefuseInvitation;

[TestFixture]
public class RefuseInvitationHandlerTests : TestBase
{
    private RefuseInvitationHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private GroupRepository _groupRepository = null!;
    private ITokenService _tokenService = null!;
    private UserGroupInvitationRepository _userGroupInvitationRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private UserPreferenceRepository _userPreferenceRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userGroupInvitationRepository = new UserGroupInvitationRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
        
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, Guid.NewGuid().ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidInvitation_ShouldRefuse()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup = await _groupRepository.GetGroupWithMembersAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Members.Any(m => m.UserId == user.Id).Should().BeFalse();
    }

    [Test]
    public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new RefuseInvitationCommand
        {
            UserId = Guid.NewGuid(),
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Test]
    public async Task Handle_WithUserWithoutPreferences_ShouldThrowBadRequestException()
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

        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User has not filled any preferences yet.");
    }

    [Test]
    public async Task Handle_WithInvalidToken_ShouldThrowUnauthorizedException()
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

        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((false, string.Empty));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "invalid-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid invitation token.");
    }

    [Test]
    public async Task Handle_WithExpiredToken_ShouldThrowUnauthorizedException()
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

        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, Guid.NewGuid().ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((true, DateTime.UtcNow.AddDays(-1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "expired-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invitation token has expired.");
    }

    [Test]
    public async Task Handle_WithInvalidInvitationId_ShouldThrowNotFoundException()
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

        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, "invalid-guid"));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Invitation not found.");
    }

    [Test]
    public async Task Handle_WithNonExistentInvitation_ShouldThrowNotFoundException()
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

        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var nonExistentInvitationId = Guid.NewGuid();
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, nonExistentInvitationId.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Invitation not found.");
    }

    [Test]
    public async Task Handle_WithInactiveInvitation_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        invitation.Cancel();
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Invitation is not active.");
    }

    [Test]
    public async Task Handle_WithExpiredInvitation_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        typeof(Domain.Aggregates.GroupAggregate.Entities.GroupInvitation).GetProperty("ExpirationDate")!.SetValue(invitation, DateTime.UtcNow.AddDays(-1));
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Invitation is not active.");
    }

    [Test]
    public async Task Handle_WithAlreadyAnsweredInvitation_ShouldThrowConflictException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        invitation.AddAnswer(user.Id, isAccepted: false);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("You have already answered this invitation.");
    }

    [Test]
    public async Task Handle_WithAlreadyMember_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.AddMember(user, isOwner: false);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new RefuseInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new RefuseInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("You are already a member of this group.");
    }
}
