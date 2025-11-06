using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Behaviours;
using MediatR;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviours;

[TestFixture]
public class ValidationBehaviourTests
{
    private class TestRequest : IRequest<TestResponse>
    {
        public string Value { get; set; } = string.Empty;
    }

    private class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }

    private class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage("Value is required");
        }
    }
    
    [Test]
    public async Task Handle_WithValidRequest_ShouldProceedToNext()
    {
        // Arrange
        var validator = new TestRequestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = "Valid" };
        var response = new TestResponse { Result = "Success" };
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(response);

        // Act
        var result = await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
    }

    [Test]
    public void Handle_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var validator = new TestRequestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = "" };
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(new TestResponse());

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () =>
            await behaviour.Handle(request, next, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WithNoValidators_ShouldProceedToNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        var behaviour = new ValidationBehaviour<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = "Any" };
        var response = new TestResponse { Result = "Success" };
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(response);

        // Act
        var result = await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
    }
}
