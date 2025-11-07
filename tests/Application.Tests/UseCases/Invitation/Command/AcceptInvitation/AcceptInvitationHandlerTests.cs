using Application.Tests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.Tests.UseCases.Invitation.Command.AcceptInvitation;

[TestFixture]
public class AcceptInvitationHandlerTests : TestBase
{
    private AcceptInvitationHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private GroupPreferenceRepository _groupPreferenceRepository = null!;
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
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _groupPreferenceRepository = new GroupPreferenceRepository(DbContext);
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
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidInvitation_ShouldAcceptAndAddMember()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var ownerPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        owner.SetPreferences(ownerPrefs);
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(owner.Preferences!, CancellationToken.None);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.UpdatePreferences(ownerPrefs);
        
        await _groupRepository.AddAsync(group, CancellationToken.None);
        var groupPrefs = group.Preferences;
        await _groupPreferenceRepository.AddAsync(groupPrefs, CancellationToken.None);
        
        var invitation = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup = await _groupRepository.GetGroupWithMembersAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Members.Any(m => m.UserId == user.Id).Should().BeTrue();
    }

    [Test]
    public async Task Handle_WithInvalidToken_ShouldThrowUnauthorizedException()
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
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var prefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(prefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((false, string.Empty));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "invalid-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.UnauthorizedException>();
    }

    [Test]
    public async Task Handle_WithExpiredToken_ShouldThrowUnauthorizedException()
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
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var prefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(prefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, Guid.NewGuid().ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((true, DateTime.UtcNow.AddDays(-1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "expired-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.UnauthorizedException>();
    }

    [Test]
    public async Task Handle_WithUserWithoutPreferences_ShouldThrowBadRequestException()
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
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>()
            .WithMessage("User has not filled any preferences yet.");
    }

    [Test]
    public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AcceptInvitationCommand
        {
            UserId = Guid.NewGuid(),
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>()
            .WithMessage("User not found.");
    }

    [Test]
    public async Task Handle_WithExpiredInvitation_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        // Set expiration date to past using reflection
        var expirationDateField = typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation)
            .GetField("_expirationDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        expirationDateField!.SetValue(invitation, DateTime.UtcNow.AddDays(-1));
        
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>()
            .WithMessage("Invitation is not active.");
    }

    [Test]
    public async Task Handle_WithInvalidInvitationId_ShouldThrowNotFoundException()
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
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var prefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(prefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, "invalid-guid"));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>()
            .WithMessage("Invitation not found.");
    }

    [Test]
    public async Task Handle_WithInactiveInvitation_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        invitation.Cancel();
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>()
            .WithMessage("Invitation is not active.");
    }

    [Test]
    public async Task Handle_WithAlreadyAnsweredInvitation_ShouldThrowConflictException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        invitation.AddAnswer(user.Id, isAccepted: true);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.ConflictException>()
            .WithMessage("You have already answered this invitation.");
    }

    [Test]
    public async Task Handle_WithAlreadyMember_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var userEmail = TestDataHelper.GenerateRandomEmail();
        var userPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, userEmail, userPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        user.SetPreferences(userPrefs);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.AddMember(user, isOwner: false);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        
        var invitation = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.ValidateInvitationToken(It.IsAny<string>()))
            .Returns((true, invitation.Id.ToString()));
        tokenServiceMock.Setup(x => x.IsTokenExpired(It.IsAny<string>()))
            .Returns((false, DateTime.UtcNow.AddDays(1)));
        _tokenService = tokenServiceMock.Object;
        
        _handler = new AcceptInvitationHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _groupInvitationRepository,
            _tokenService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository);

        var command = new AcceptInvitationCommand
        {
            UserId = user.Id,
            Token = "test-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>()
            .WithMessage("You are already a member of this group.");
    }
}
