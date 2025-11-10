using Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

[TestFixture]
public class VoteAtDestinationForGroupIdValidatorTests
{
    private VoteAtDestinationForGroupIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new VoteAtDestinationForGroupIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new VoteAtDestinationForGroupIdCommand { GroupId = Guid.NewGuid(), DestinationId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsApproved = true };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
