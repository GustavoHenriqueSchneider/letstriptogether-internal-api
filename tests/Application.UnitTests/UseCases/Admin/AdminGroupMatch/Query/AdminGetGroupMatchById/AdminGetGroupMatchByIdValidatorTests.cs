using Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

[TestFixture]
public class AdminGetGroupMatchByIdValidatorTests
{
    private AdminGetGroupMatchByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetGroupMatchByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetGroupMatchByIdQuery { GroupId = Guid.NewGuid(), MatchId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
