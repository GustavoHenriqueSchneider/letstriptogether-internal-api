using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

[TestFixture]
public class AdminGetAllGroupMatchesByIdValidatorTests
{
    private AdminGetAllGroupMatchesByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllGroupMatchesByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllGroupMatchesByIdQuery { GroupId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
