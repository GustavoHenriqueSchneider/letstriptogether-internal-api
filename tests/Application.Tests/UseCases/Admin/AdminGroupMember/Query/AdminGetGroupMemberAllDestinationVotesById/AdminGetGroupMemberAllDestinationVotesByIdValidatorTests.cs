using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

[TestFixture]
public class AdminGetGroupMemberAllDestinationVotesByIdValidatorTests
{
    private AdminGetGroupMemberAllDestinationVotesByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetGroupMemberAllDestinationVotesByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetGroupMemberAllDestinationVotesByIdQuery { GroupId = Guid.NewGuid(), MemberId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
