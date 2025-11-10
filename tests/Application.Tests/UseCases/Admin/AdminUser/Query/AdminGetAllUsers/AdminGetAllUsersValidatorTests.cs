using Application.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;

[TestFixture]
public class AdminGetAllUsersValidatorTests
{
    private AdminGetAllUsersValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllUsersValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllUsersQuery { PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
