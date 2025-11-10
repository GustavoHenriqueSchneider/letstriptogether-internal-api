using Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;

[TestFixture]
public class GetActiveGroupInvitationValidatorTests
{
    private GetActiveGroupInvitationValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new GetActiveGroupInvitationValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetActiveGroupInvitationQuery { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
