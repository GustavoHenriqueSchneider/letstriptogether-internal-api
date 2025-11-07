using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

[TestFixture]
public class AdminAnonymizeUserByIdValidatorTests
{
    private AdminAnonymizeUserByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminAnonymizeUserByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminAnonymizeUserByIdCommand { UserId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
