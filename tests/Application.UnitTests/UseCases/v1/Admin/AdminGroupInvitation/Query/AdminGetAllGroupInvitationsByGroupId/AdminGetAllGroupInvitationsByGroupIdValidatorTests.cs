using Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

[TestFixture]
public class AdminGetAllGroupInvitationsByGroupIdValidatorTests
{
    private AdminGetAllGroupInvitationsByGroupIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllGroupInvitationsByGroupIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllGroupInvitationsByGroupIdQuery { GroupId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
