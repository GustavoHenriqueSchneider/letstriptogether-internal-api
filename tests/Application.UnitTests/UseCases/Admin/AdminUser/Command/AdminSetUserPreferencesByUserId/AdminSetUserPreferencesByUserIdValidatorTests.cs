using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

[TestFixture]
public class AdminSetUserPreferencesByUserIdValidatorTests
{
    private AdminSetUserPreferencesByUserIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminSetUserPreferencesByUserIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminSetUserPreferencesByUserIdCommand { UserId = Guid.NewGuid(), LikesCommercial = true, Food = new List<string>(), Culture = new List<string>(), Entertainment = new List<string>(), PlaceTypes = new List<string>() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
