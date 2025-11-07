using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;

[TestFixture]
public class AdminGetAllDestinationsValidatorTests
{
    private AdminGetAllDestinationsValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminGetAllDestinationsValidator();
    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new AdminGetAllDestinationsQuery { PageNumber = 1, PageSize = 10 };
        _validator.Validate(query).IsValid.Should().BeTrue();
    }
}
