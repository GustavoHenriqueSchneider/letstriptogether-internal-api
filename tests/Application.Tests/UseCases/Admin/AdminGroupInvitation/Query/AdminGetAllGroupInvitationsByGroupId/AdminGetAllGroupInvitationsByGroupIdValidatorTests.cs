using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

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
