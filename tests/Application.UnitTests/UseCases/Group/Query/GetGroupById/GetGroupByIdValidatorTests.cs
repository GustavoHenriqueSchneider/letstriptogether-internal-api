using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetGroupById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Group.Query.GetGroupById;

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
