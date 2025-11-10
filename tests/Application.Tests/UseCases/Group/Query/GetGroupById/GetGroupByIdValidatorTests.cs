using Application.UseCases.Group.Query.GetGroupById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Group.Query.GetGroupById;

[TestFixture]
public class GetGroupByIdValidatorTests
{
    private GetGroupByIdValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new GetGroupByIdValidator();

    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetGroupByIdQuery { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }
}
