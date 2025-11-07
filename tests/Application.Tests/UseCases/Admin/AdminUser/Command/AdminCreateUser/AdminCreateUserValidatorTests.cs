using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Command.AdminCreateUser;

[TestFixture]
public class AdminCreateUserValidatorTests
{
    private AdminCreateUserValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminCreateUserValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminCreateUserCommand { Name = "Test User", Email = "test@example.com", Password = "ValidPass123!" };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
