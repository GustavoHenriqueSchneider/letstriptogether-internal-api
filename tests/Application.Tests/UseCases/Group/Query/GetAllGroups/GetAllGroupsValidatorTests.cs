using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;
using NUnit.Framework;

namespace Application.Tests.UseCases.Group.Query.GetAllGroups;

[TestFixture]
public class GetAllGroupsValidatorTests
{
    private GetAllGroupsValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new GetAllGroupsValidator();

    [Test]
    public void Validate_WithValidQuery_ShouldReturnValid()
    {
        var query = new GetAllGroupsQuery { UserId = Guid.NewGuid(), PageNumber = 1, PageSize = 10 };
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }
}
