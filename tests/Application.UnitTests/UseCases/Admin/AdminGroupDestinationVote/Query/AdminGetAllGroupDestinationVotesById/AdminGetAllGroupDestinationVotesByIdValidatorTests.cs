using Application.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

[TestFixture]
public class AdminGetAllGroupDestinationVotesByIdValidatorTests
{
    private AdminGetAllGroupDestinationVotesByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllGroupDestinationVotesByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllGroupDestinationVotesByIdQuery { GroupId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
