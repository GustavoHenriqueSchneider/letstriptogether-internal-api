using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Domain;

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
}
