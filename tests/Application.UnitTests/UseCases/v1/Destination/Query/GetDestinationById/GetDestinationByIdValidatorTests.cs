using Application.UseCases.v1.Destination.Query.GetDestinationById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Destination.Query.GetDestinationById;

[TestFixture]
public class GetDestinationByIdValidatorTests
{
    private GetDestinationByIdValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new GetDestinationByIdValidator();

    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetDestinationByIdQuery { DestinationId = Guid.NewGuid() };
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }
}
