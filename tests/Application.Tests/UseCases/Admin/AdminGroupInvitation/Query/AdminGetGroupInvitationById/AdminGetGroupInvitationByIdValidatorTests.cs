using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

[TestFixture]
public class AdminGetGroupInvitationByIdValidatorTests
{
    private AdminGetGroupInvitationByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetGroupInvitationByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetGroupInvitationByIdQuery { GroupId = Guid.NewGuid(), InvitationId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
