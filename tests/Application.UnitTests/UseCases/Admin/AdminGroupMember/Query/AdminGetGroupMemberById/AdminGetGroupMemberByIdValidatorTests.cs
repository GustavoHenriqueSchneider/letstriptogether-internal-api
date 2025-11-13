using Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

[TestFixture]
public class AdminGetGroupMemberByIdValidatorTests
{
    private AdminGetGroupMemberByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetGroupMemberByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetGroupMemberByIdQuery { GroupId = Guid.NewGuid(), MemberId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
