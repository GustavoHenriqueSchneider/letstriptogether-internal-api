using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

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
