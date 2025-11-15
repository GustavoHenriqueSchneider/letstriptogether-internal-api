using Application.UseCases.v1.Group.Command.LeaveGroupById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Group.Command.LeaveGroupById;

[TestFixture]
public class LeaveGroupByIdValidatorTests
{
    private LeaveGroupByIdValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new LeaveGroupByIdValidator();

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new LeaveGroupByIdCommand { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
