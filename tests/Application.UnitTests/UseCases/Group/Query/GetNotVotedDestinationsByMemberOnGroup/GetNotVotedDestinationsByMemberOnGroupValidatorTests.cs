using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

[TestFixture]
public class GetNotVotedDestinationsByMemberOnGroupValidatorTests
{
    private GetNotVotedDestinationsByMemberOnGroupValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new GetNotVotedDestinationsByMemberOnGroupValidator();

    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetNotVotedDestinationsByMemberOnGroupQuery { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }
}
