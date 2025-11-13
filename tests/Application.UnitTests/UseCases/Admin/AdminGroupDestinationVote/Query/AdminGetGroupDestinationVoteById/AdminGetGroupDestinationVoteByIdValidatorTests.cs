using Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

[TestFixture]
public class AdminGetGroupDestinationVoteByIdValidatorTests
{
    private AdminGetGroupDestinationVoteByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetGroupDestinationVoteByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetGroupDestinationVoteByIdQuery { GroupId = Guid.NewGuid(), DestinationVoteId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
