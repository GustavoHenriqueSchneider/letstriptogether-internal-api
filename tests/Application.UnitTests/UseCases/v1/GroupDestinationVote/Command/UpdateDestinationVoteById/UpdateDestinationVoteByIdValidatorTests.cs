using Application.UseCases.v1.GroupDestinationVote.Command.UpdateDestinationVoteById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.GroupDestinationVote.Command.UpdateDestinationVoteById;

[TestFixture]
public class UpdateDestinationVoteByIdValidatorTests
{
    private UpdateDestinationVoteByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new UpdateDestinationVoteByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new UpdateDestinationVoteByIdCommand { GroupId = Guid.NewGuid(), DestinationVoteId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsApproved = true };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
