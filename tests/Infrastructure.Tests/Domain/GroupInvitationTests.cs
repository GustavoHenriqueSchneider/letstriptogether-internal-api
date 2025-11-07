using FluentAssertions;
using Infrastructure.Tests.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using NUnit.Framework;

namespace Infrastructure.Tests.Domain;

[TestFixture]
public class GroupInvitationTests : TestBase
{
    [Test]
    public void Expire_WithActiveStatus_ShouldSetStatusToExpired()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);

        // Act
        invitation.Expire();

        // Assert
        invitation.Status.Should().Be(GroupInvitationStatus.Expired);
    }

    [Test]
    public void Expire_WithCancelledStatus_ShouldThrowException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        invitation.Cancel();

        // Act & Assert
        var act = () => invitation.Expire();
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("It is not possible to expire a cancelled group invitation.");
    }

    [Test]
    public void Expire_WithExpiredStatus_ShouldThrowException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        invitation.Expire();

        // Act & Assert
        var act = () => invitation.Expire();
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("It is not possible to expire a already expired group invitation.");
    }

    [Test]
    public void Cancel_WithActiveStatus_ShouldSetStatusToCancelled()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);

        // Act
        invitation.Cancel();

        // Assert
        invitation.Status.Should().Be(GroupInvitationStatus.Cancelled);
    }

    [Test]
    public void Cancel_WithCancelledStatus_ShouldThrowException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        invitation.Cancel();

        // Act & Assert
        var act = () => invitation.Cancel();
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("It is not possible to cancel a already cancelled group invitation.");
    }

    [Test]
    public void Cancel_WithExpiredStatus_ShouldThrowException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        invitation.Expire();

        // Act & Assert
        var act = () => invitation.Cancel();
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("It is not possible to cancel a expired group invitation.");
    }

    [Test]
    public void Copy_WithActiveInvitation_ShouldCancelOriginalAndCreateNew()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var originalInvitation = new GroupInvitation(groupId);

        // Act
        var newInvitation = originalInvitation.Copy();

        // Assert
        originalInvitation.Status.Should().Be(GroupInvitationStatus.Cancelled);
        newInvitation.Should().NotBeNull();
        newInvitation.GroupId.Should().Be(groupId);
        newInvitation.Status.Should().Be(GroupInvitationStatus.Active);
    }

    [Test]
    public void AddAnswer_WithNewUser_ShouldAddAnswer()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        var userId = Guid.NewGuid();

        // Act
        var answer = invitation.AddAnswer(userId, isAccepted: true);

        // Assert
        answer.Should().NotBeNull();
        answer.UserId.Should().Be(userId);
        answer.IsAccepted.Should().BeTrue();
        answer.GroupInvitationId.Should().Be(invitation.Id);
        invitation.AnsweredBy.Should().HaveCount(1);
        invitation.AnsweredBy.Should().Contain(answer);
    }

    [Test]
    public void AddAnswer_WithDuplicateUser_ShouldThrowException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        var userId = Guid.NewGuid();
        invitation.AddAnswer(userId, isAccepted: true);

        // Act & Assert
        var act = () => invitation.AddAnswer(userId, isAccepted: false);
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("This answer is already included on the invitation.");
    }

    [Test]
    public void AddAnswer_WithDifferentUsers_ShouldAddMultipleAnswers()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitation = new GroupInvitation(groupId);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var answer1 = invitation.AddAnswer(userId1, isAccepted: true);
        var answer2 = invitation.AddAnswer(userId2, isAccepted: false);

        // Assert
        invitation.AnsweredBy.Should().HaveCount(2);
        invitation.AnsweredBy.Should().Contain(answer1);
        invitation.AnsweredBy.Should().Contain(answer2);
    }
}
