using FluentAssertions;
using System.Reflection;
using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common.Exceptions;
using Domain.Security;
using Infrastructure.Tests.Common;
using NUnit.Framework;

namespace Infrastructure.Tests.Domain;

[TestFixture]
public class GroupTests : TestBase
{
    [Test]
    public void HasMatch_WithExistingMatch_ShouldReturnTrue()
    {
        // Arrange
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var destinationId = Guid.NewGuid();
        
        var matchesField = typeof(Group).GetField("_matches", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var matchesList = (System.Collections.Generic.List<GroupMatch>)matchesField!.GetValue(group)!;
        
        var match = new GroupMatch
        {
            GroupId = group.Id,
            DestinationId = destinationId
        };
        matchesList.Add(match);

        // Act
        var result = group.HasMatch(match);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void HasMatch_WithoutMatch_ShouldReturnFalse()
    {
        // Arrange
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var match = new GroupMatch
        {
            GroupId = group.Id,
            DestinationId = Guid.NewGuid()
        };

        // Act
        var result = group.HasMatch(match);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CreateMatch_WithOnlyOneMember_ShouldThrowDomainBusinessRuleException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var user = new User("Test User", "test@example.com", "Password123!", role);
        group.AddMember(user, true);
        var destinationId = Guid.NewGuid();

        // Act
        var act = () => group.CreateMatch(destinationId);

        // Assert
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("It is not possible to create a group match with only one member.");
    }

    [Test]
    public void CreateMatch_WhenNotAllMembersAgreed_ShouldThrowDomainBusinessRuleException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var user1 = new User("User 1", "user1@example.com", "Password123!", role);
        var user2 = new User("User 2", "user2@example.com", "Password123!", role);
        var member1 = group.AddMember(user1, true);
        var member2 = group.AddMember(user2, false);
        var destinationId = Guid.NewGuid();

        // Add vote only for member1 (approved)
        var votesField1 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList1 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField1!.GetValue(member1)!;
        votesList1.Add(new GroupMemberDestinationVote(member1.Id, destinationId, true));

        // Add vote for member2 but not approved
        var votesField2 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList2 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField2!.GetValue(member2)!;
        votesList2.Add(new GroupMemberDestinationVote(member2.Id, destinationId, false));

        // Act
        var act = () => group.CreateMatch(destinationId);

        // Assert
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("Not all group members agreed with the informed destination.");
    }

    [Test]
    public void CreateMatch_WhenMemberHasNoVote_ShouldThrowDomainBusinessRuleException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var user1 = new User("User 1", "user1@example.com", "Password123!", role);
        var user2 = new User("User 2", "user2@example.com", "Password123!", role);
        var member1 = group.AddMember(user1, true);
        group.AddMember(user2, false);
        var destinationId = Guid.NewGuid();

        // Add vote only for member1
        var votesField1 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList1 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField1!.GetValue(member1)!;
        votesList1.Add(new GroupMemberDestinationVote(member1.Id, destinationId, true));

        // Act
        var act = () => group.CreateMatch(destinationId);

        // Assert
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("Not all group members agreed with the informed destination.");
    }

    [Test]
    public void CreateMatch_WhenMatchAlreadyExists_ShouldThrowDomainBusinessRuleException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var user1 = new User("User 1", "user1@example.com", "Password123!", role);
        var user2 = new User("User 2", "user2@example.com", "Password123!", role);
        var member1 = group.AddMember(user1, true);
        var member2 = group.AddMember(user2, false);
        var destinationId = Guid.NewGuid();

        // Add approved votes for both members
        var votesField1 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList1 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField1!.GetValue(member1)!;
        votesList1.Add(new GroupMemberDestinationVote(member1.Id, destinationId, true));

        var votesField2 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList2 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField2!.GetValue(member2)!;
        votesList2.Add(new GroupMemberDestinationVote(member2.Id, destinationId, true));

        // Create match first time
        group.CreateMatch(destinationId);

        // Act
        var act = () => group.CreateMatch(destinationId);

        // Assert
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("This match is already included on the group.");
    }

    [Test]
    public void CreateMatch_WithAllMembersAgreed_ShouldCreateMatch()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var user1 = new User("User 1", "user1@example.com", "Password123!", role);
        var user2 = new User("User 2", "user2@example.com", "Password123!", role);
        var member1 = group.AddMember(user1, true);
        var member2 = group.AddMember(user2, false);
        var destinationId = Guid.NewGuid();

        // Add approved votes for both members
        var votesField1 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList1 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField1!.GetValue(member1)!;
        votesList1.Add(new GroupMemberDestinationVote(member1.Id, destinationId, true));

        var votesField2 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList2 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField2!.GetValue(member2)!;
        votesList2.Add(new GroupMemberDestinationVote(member2.Id, destinationId, true));

        // Act
        var result = group.CreateMatch(destinationId);

        // Assert
        result.Should().NotBeNull();
        result.GroupId.Should().Be(group.Id);
        result.DestinationId.Should().Be(destinationId);
        group.Matches.Should().Contain(result);
        group.Matches.Count.Should().Be(1);
    }

    [Test]
    public void CreateMatch_WithThreeMembersAllAgreed_ShouldCreateMatch()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var user1 = new User("User 1", "user1@example.com", "Password123!", role);
        var user2 = new User("User 2", "user2@example.com", "Password123!", role);
        var user3 = new User("User 3", "user3@example.com", "Password123!", role);
        var member1 = group.AddMember(user1, true);
        var member2 = group.AddMember(user2, false);
        var member3 = group.AddMember(user3, false);
        var destinationId = Guid.NewGuid();

        // Add approved votes for all members
        var votesField1 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList1 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField1!.GetValue(member1)!;
        votesList1.Add(new GroupMemberDestinationVote(member1.Id, destinationId, true));

        var votesField2 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList2 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField2!.GetValue(member2)!;
        votesList2.Add(new GroupMemberDestinationVote(member2.Id, destinationId, true));

        var votesField3 = typeof(GroupMember).GetField("_votes", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var votesList3 = (System.Collections.Generic.List<GroupMemberDestinationVote>)votesField3!.GetValue(member3)!;
        votesList3.Add(new GroupMemberDestinationVote(member3.Id, destinationId, true));

        // Act
        var result = group.CreateMatch(destinationId);

        // Assert
        result.Should().NotBeNull();
        result.GroupId.Should().Be(group.Id);
        result.DestinationId.Should().Be(destinationId);
        group.Matches.Should().Contain(result);
        group.Matches.Count.Should().Be(1);
    }
}
