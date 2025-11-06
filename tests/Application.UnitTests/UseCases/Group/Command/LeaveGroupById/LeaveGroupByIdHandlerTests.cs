using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.LeaveGroupById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Group.Command.LeaveGroupById;

[TestFixture]
public class LeaveGroupByIdHandlerTests : TestBase
{
    private LeaveGroupByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMatchRepository _groupMatchRepository = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private GroupPreferenceRepository _groupPreferenceRepository = null!;
    private GroupRepository _groupRepository = null!;
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
        _groupMatchRepository = new GroupMatchRepository(DbContext);
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _groupPreferenceRepository = new GroupPreferenceRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
        
        _handler = new LeaveGroupByIdHandler(
            _groupMatchRepository,
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _unitOfWork,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithMember_ShouldRemoveMember()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var memberEmail = TestDataHelper.GenerateRandomEmail();
        var memberPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var memberName = TestDataHelper.GenerateRandomName();
        var member = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(memberName, memberEmail, memberPasswordHash, role);
        await _userRepository.AddAsync(member, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.AddMember(member, isOwner: false);
        
        var preferences = new UserPreference(
            likesCommercial: true,
            food: new List<string> { new TripPreference(TripPreference.Food.Restaurant) },
            culture: new List<string> { new TripPreference(TripPreference.Culture.Museum) },
            entertainment: new List<string> { new TripPreference(TripPreference.Entertainment.Attraction) },
            placeTypes: new List<string> { new TripPreference(TripPreference.PlaceType.Beach) });
        
        owner.SetPreferences(preferences);
        member.SetPreferences(preferences);
        await _userPreferenceRepository.AddAsync(owner.Preferences!, CancellationToken.None);
        await _userPreferenceRepository.AddAsync(member.Preferences!, CancellationToken.None);
        
        group.UpdatePreferences(owner.Preferences!);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new LeaveGroupByIdCommand
        {
            UserId = member.Id,
            GroupId = group.Id
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var groupWithMembers = await _groupRepository.GetGroupWithMembersAsync(group.Id, CancellationToken.None);
        groupWithMembers.Should().NotBeNull();
        groupWithMembers!.Members.Should().HaveCount(1);
        groupWithMembers.Members.Any(m => m.UserId == member.Id).Should().BeFalse();
    }

    [Test]
    public async Task Handle_WithOwner_ShouldThrowBadRequestException()
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

        var command = new LeaveGroupByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>();
    }
}
