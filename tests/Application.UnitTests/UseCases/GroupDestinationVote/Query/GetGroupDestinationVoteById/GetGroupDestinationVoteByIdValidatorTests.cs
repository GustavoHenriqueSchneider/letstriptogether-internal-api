using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;

[TestFixture]
public class GetGroupDestinationVoteByIdValidatorTests
{
    private GetGroupDestinationVoteByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetGroupDestinationVoteByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetGroupDestinationVoteByIdQuery { GroupId = Guid.NewGuid(), DestinationVoteId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
