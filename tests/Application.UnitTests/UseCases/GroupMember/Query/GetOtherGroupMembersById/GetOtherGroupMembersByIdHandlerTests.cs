using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMember.Query.GetOtherGroupMembersById;

[TestFixture]
public class GetOtherGroupMembersByIdHandlerTests : TestBase
{
    private GetOtherGroupMembersByIdHandler _handler = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new GetOtherGroupMembersByIdHandler(_groupMemberRepository, _groupRepository, _userRepository);
    }

    [Test]
    public async Task Handle_WithMembers_ShouldReturnOtherMembers()
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
        
        var member1Email = TestDataHelper.GenerateRandomEmail();
        var member1PasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var member1Name = TestDataHelper.GenerateRandomName();
        var member1 = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(member1Name, member1Email, member1PasswordHash, role);
        await _userRepository.AddAsync(member1, CancellationToken.None);
        
        var member2Email = TestDataHelper.GenerateRandomEmail();
        var member2PasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var member2Name = TestDataHelper.GenerateRandomName();
        var member2 = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(member2Name, member2Email, member2PasswordHash, role);
        await _userRepository.AddAsync(member2, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.AddMember(member1, isOwner: false);
        group.AddMember(member2, isOwner: false);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetOtherGroupMembersByIdQuery
        {
            UserId = owner.Id,
            GroupId = group.Id,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Hits.Should().Be(2);
    }

    [Test]
    public async Task Handle_WithNonMember_ShouldThrowBadRequestException()
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
        
        var nonMemberEmail = TestDataHelper.GenerateRandomEmail();
        var nonMemberPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var nonMemberName = TestDataHelper.GenerateRandomName();
        var nonMember = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(nonMemberName, nonMemberEmail, nonMemberPasswordHash, role);
        await _userRepository.AddAsync(nonMember, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetOtherGroupMembersByIdQuery
        {
            UserId = nonMember.Id,
            GroupId = group.Id,
            PageNumber = 1,
            PageSize = 10
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>();
    }
}
