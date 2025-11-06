using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMatch.Command.RemoveGroupMatchById;

[TestFixture]
public class RemoveGroupMatchByIdValidatorTests
{
    private RemoveGroupMatchByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new RemoveGroupMatchByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new RemoveGroupMatchByIdCommand { GroupId = Guid.NewGuid(), MatchId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
