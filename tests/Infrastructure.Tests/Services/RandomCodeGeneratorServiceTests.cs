using FluentAssertions;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Infrastructure.Tests.Services;

[TestFixture]
public class RandomCodeGeneratorServiceTests
{
    private RandomCodeGeneratorService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new RandomCodeGeneratorService();
    }

    [Test]
    public void Generate_ShouldReturnSixDigitCode()
    {
        // Act
        var code = _service.Generate();

        // Assert
        code.Should().NotBeNullOrEmpty();
        code.Length.Should().Be(6);
        int.Parse(code).Should().BeInRange(100000, 999999);
    }

    [Test]
    public void Generate_MultipleTimes_ShouldReturnDifferentCodes()
    {
        // Act
        var codes = Enumerable.Range(0, 10).Select(_ => _service.Generate()).ToList();

        // Assert
        codes.Should().OnlyHaveUniqueItems();
    }

    [Test]
    public void Generate_ShouldReturnNumericString()
    {
        // Act
        var code = _service.Generate();

        // Assert
        code.Should().MatchRegex(@"^\d{6}$");
    }
}
