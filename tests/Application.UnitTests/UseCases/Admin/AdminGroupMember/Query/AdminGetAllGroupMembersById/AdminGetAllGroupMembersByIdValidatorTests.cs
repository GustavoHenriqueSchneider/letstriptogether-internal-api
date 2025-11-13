using Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;

[TestFixture]
public class AdminGetAllGroupMembersByIdValidatorTests
{
    private AdminGetAllGroupMembersByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllGroupMembersByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllGroupMembersByIdQuery { GroupId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
