using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;

[TestFixture]
public class AdminGetGroupByIdValidatorTests
{
    private AdminGetGroupByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetGroupByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetGroupByIdQuery { GroupId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
