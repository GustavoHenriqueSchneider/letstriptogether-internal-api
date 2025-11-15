using Application.UseCases.v1.GroupMember.Query.GetOtherGroupMembersById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.GroupMember.Query.GetOtherGroupMembersById;

[TestFixture]
public class GetOtherGroupMembersByIdValidatorTests
{
    private GetOtherGroupMembersByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetOtherGroupMembersByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetOtherGroupMembersByIdQuery { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
