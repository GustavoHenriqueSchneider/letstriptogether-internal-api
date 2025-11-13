using Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

[TestFixture]
public class AdminUpdateUserByIdValidatorTests
{
    private AdminUpdateUserByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminUpdateUserByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminUpdateUserByIdCommand { UserId = Guid.NewGuid(), Name = "Test User" };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
