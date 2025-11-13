using Application.UseCases.Group.Command.DeleteGroupById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Group.Command.DeleteGroupById;

[TestFixture]
public class DeleteGroupByIdValidatorTests
{
    private DeleteGroupByIdValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new DeleteGroupByIdValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new DeleteGroupByIdCommand { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
