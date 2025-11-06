using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.Register;

[TestFixture]
public class RegisterResponseTests
{
    [Test]
    public void RegisterResponse_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = new RegisterResponse
        {
            Id = id
        };

        // Assert
        response.Id.Should().Be(id);
    }
}
