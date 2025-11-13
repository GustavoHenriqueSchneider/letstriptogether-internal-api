using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;
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
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

[TestFixture]
public class AdminRemoveGroupMemberByIdHandlerTests : TestBase
{
    private AdminRemoveGroupMemberByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private GroupPreferenceRepository _groupPreferenceRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;
    private UserPreferenceRepository _userPreferenceRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _groupPreferenceRepository = new GroupPreferenceRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
        
        _handler = new AdminRemoveGroupMemberByIdHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _unitOfWork);
    }

    [Test]
    public async Task Handle_WithValidMember_ShouldRemoveMember()
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
        
        var memberEmail = TestDataHelper.GenerateRandomEmail();
        var memberPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var memberName = TestDataHelper.GenerateRandomName();
        var member = new Domain.Aggregates.UserAggregate.Entities.User(memberName, memberEmail, memberPasswordHash, role);
        await _userRepository.AddAsync(member, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var ownerPrefs = new UserPreference(true, new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, new List<string>(), new List<string>(), new List<string>());
        owner.SetPreferences(ownerPrefs);
        await _userPreferenceRepository.AddAsync(owner.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.AddMember(member, isOwner: false);
        group.UpdatePreferences(ownerPrefs);
        
        await _groupRepository.AddAsync(group, CancellationToken.None);
        var groupPrefs = group.Preferences;
        await _groupPreferenceRepository.AddAsync(groupPrefs, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var memberToRemove = group.Members.First(m => m.UserId == member.Id);
        var command = new AdminRemoveGroupMemberByIdCommand
        {
            GroupId = group.Id,
            MemberId = memberToRemove.Id
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup = await _groupRepository.GetGroupWithMembersAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Members.Any(m => m.UserId == member.Id).Should().BeFalse();
    }
}
