using Application.UseCases.GroupMember.Query.GetGroupMemberById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMember.Query.GetGroupMemberById;

[TestFixture]
public class GetGroupMemberByIdValidatorTests
{
    private GetGroupMemberByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetGroupMemberByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetGroupMemberByIdQuery { GroupId = Guid.NewGuid(), MemberId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
