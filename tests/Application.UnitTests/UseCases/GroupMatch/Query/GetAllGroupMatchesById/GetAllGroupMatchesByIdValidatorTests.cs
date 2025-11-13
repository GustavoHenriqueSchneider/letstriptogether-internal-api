using Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMatch.Query.GetAllGroupMatchesById;

[TestFixture]
public class GetAllGroupMatchesByIdValidatorTests
{
    private GetAllGroupMatchesByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetAllGroupMatchesByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetAllGroupMatchesByIdQuery { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
