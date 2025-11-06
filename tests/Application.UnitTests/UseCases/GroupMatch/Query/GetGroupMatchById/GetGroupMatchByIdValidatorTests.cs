using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetGroupMatchById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMatch.Query.GetGroupMatchById;

[TestFixture]
public class GetGroupMatchByIdValidatorTests
{
    private GetGroupMatchByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetGroupMatchByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetGroupMatchByIdQuery { GroupId = Guid.NewGuid(), MatchId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
