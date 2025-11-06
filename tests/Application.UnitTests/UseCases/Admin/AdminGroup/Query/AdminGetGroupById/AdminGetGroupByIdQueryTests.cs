using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;

[TestFixture]
public class AdminGetGroupByIdQueryTests
{
    [Test]
    public void AdminGetGroupByIdQuery_ShouldSetProperties()
    {
        // Arrange
        var groupId = Guid.NewGuid();

        // Act
        var query = new AdminGetGroupByIdQuery
        {
            GroupId = groupId
        };

        // Assert
        query.GroupId.Should().Be(groupId);
    }
}
