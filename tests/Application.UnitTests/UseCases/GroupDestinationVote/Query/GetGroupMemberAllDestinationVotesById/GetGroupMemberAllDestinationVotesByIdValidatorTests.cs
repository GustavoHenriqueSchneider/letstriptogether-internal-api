using Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

[TestFixture]
public class GetGroupMemberAllDestinationVotesByIdValidatorTests
{
    private GetGroupMemberAllDestinationVotesByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetGroupMemberAllDestinationVotesByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetGroupMemberAllDestinationVotesByIdQuery { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
