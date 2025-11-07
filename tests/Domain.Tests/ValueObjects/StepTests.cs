using FluentAssertions;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.ValueObjects;
using NUnit.Framework;

namespace Domain.Tests.ValueObjects;

[TestFixture]
public class StepTests
{
    [Test]
    public void Constructor_WithValidStep_ShouldCreateInstance()
    {
        // Act
        var step = new Step(Step.ValidateEmail);

        // Assert
        step.Should().NotBeNull();
        step.ToString().Should().Be(Step.ValidateEmail);
    }

    [Test]
    public void Constructor_WithSetPassword_ShouldCreateInstance()
    {
        // Act
        var step = new Step(Step.SetPassword);

        // Assert
        step.Should().NotBeNull();
        step.ToString().Should().Be(Step.SetPassword);
    }

    [Test]
    public void Constructor_WithInvalidStep_ShouldThrowDomainBusinessRuleException()
    {
        // Act
        var act = () => new Step("invalid-step");

        // Assert
        act.Should().Throw<DomainBusinessRuleException>()
            .WithMessage("Invalid step: invalid-step");
    }

    [Test]
    public void ToString_ShouldReturnStepValue()
    {
        // Arrange
        var step = new Step(Step.ValidateEmail);

        // Act
        var result = step.ToString();

        // Assert
        result.Should().Be(Step.ValidateEmail);
    }

    [Test]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var step = new Step(Step.SetPassword);

        // Act
        string result = step;

        // Assert
        result.Should().Be(Step.SetPassword);
    }
}
