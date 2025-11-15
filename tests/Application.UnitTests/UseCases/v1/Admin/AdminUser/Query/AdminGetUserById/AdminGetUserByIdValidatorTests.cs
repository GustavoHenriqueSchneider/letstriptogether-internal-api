using Application.UseCases.v1.Admin.AdminUser.Query.AdminGetUserById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminUser.Query.AdminGetUserById;

[TestFixture]
public class AdminGetUserByIdValidatorTests
{
    private AdminGetUserByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetUserByIdValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetUserByIdQuery { UserId = Guid.NewGuid() };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
