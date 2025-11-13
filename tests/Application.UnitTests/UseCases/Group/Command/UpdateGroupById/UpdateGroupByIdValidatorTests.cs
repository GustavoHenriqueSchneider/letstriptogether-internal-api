using Application.UseCases.Group.Command.UpdateGroupById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Group.Command.UpdateGroupById;

[TestFixture]
public class UpdateGroupByIdValidatorTests
{
    private UpdateGroupByIdValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new UpdateGroupByIdValidator();

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new UpdateGroupByIdCommand { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
