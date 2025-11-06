using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;

[TestFixture]
public class AdminGetAllGroupsValidatorTests
{
    private AdminGetAllGroupsValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllGroupsValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllGroupsQuery { PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
