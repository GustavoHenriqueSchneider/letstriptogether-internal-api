using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

[TestFixture]
public class AdminDeleteUserByIdValidatorTests
{
    private AdminDeleteUserByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminDeleteUserByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminDeleteUserByIdCommand { UserId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
