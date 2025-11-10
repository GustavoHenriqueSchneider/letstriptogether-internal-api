using Domain.Aggregates.DestinationAggregate.Entities;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using Infrastructure.Tests.Common;
using NUnit.Framework;

namespace Infrastructure.Tests.Domain;

[TestFixture]
public class DestinationTests : TestBase
{
    [Test]
    public void HasCommercialCategory_WithShoppingAttraction_ShouldReturnTrue()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        
        var attractionsField = typeof(Destination).GetField("_attractions", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attraction = new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Shopping Mall",
            Description = "A shopping mall",
            Category = TripPreference.Shopping
        };
        var attractionsList = (System.Collections.Generic.List<DestinationAttraction>)attractionsField!.GetValue(destination)!;
        attractionsList.Add(attraction);

        // Act
        var result = destination.HasCommercialCategory();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void HasCommercialCategory_WithoutShoppingAttraction_ShouldReturnFalse()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };

        // Act
        var result = destination.HasCommercialCategory();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetCultureCategories_WithCultureAttractions_ShouldReturnCategories()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        
        var attractionsField = typeof(Destination).GetField("_attractions", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attractionsList = (System.Collections.Generic.List<DestinationAttraction>)attractionsField!.GetValue(destination)!;
        
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Museum",
            Description = "A museum",
            Category = "culture.museum"
        });
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Theater",
            Description = "A theater",
            Category = "culture.theater"
        });

        // Act
        var result = destination.GetCultureCategories();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("culture.museum");
        result.Should().Contain("culture.theater");
    }

    [Test]
    public void GetCultureCategories_WithoutCultureAttractions_ShouldReturnEmpty()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };

        // Act
        var result = destination.GetCultureCategories();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetEntertainmentCategories_WithEntertainmentAttractions_ShouldReturnCategories()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        
        var attractionsField = typeof(Destination).GetField("_attractions", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attractionsList = (System.Collections.Generic.List<DestinationAttraction>)attractionsField!.GetValue(destination)!;
        
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Nightclub",
            Description = "A nightclub",
            Category = "entertainment.nightclub"
        });
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Bar",
            Description = "A bar",
            Category = "entertainment.bar"
        });

        // Act
        var result = destination.GetEntertainmentCategories();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("entertainment.nightclub");
        result.Should().Contain("entertainment.bar");
    }

    [Test]
    public void GetEntertainmentCategories_WithoutEntertainmentAttractions_ShouldReturnEmpty()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };

        // Act
        var result = destination.GetEntertainmentCategories();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetFoodCategories_WithFoodAttractions_ShouldReturnCategories()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        
        var attractionsField = typeof(Destination).GetField("_attractions", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attractionsList = (System.Collections.Generic.List<DestinationAttraction>)attractionsField!.GetValue(destination)!;
        
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Restaurant",
            Description = "A restaurant",
            Category = "food.restaurant"
        });
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Cafe",
            Description = "A cafe",
            Category = "food.cafe"
        });

        // Act
        var result = destination.GetFoodCategories();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("food.restaurant");
        result.Should().Contain("food.cafe");
    }

    [Test]
    public void GetPlaceTypes_WithPlaceTypeAttractions_ShouldReturnCategories()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        
        var attractionsField = typeof(Destination).GetField("_attractions", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attractionsList = (System.Collections.Generic.List<DestinationAttraction>)attractionsField!.GetValue(destination)!;
        
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Park",
            Description = "A park",
            Category = "placetype.park"
        });
        attractionsList.Add(new DestinationAttraction
        {
            DestinationId = destination.Id,
            Name = "Beach",
            Description = "A beach",
            Category = "placetype.beach"
        });

        // Act
        var result = destination.GetPlaceTypes();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("placetype.park");
        result.Should().Contain("placetype.beach");
    }

    [Test]
    public void GroupMatches_Getter_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };

        // Act
        var groupMatches = destination.GroupMatches;

        // Assert
        groupMatches.Should().NotBeNull();
        groupMatches.Should().BeEmpty();
    }

    [Test]
    public void GroupMemberVotes_Getter_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };

        // Act
        var groupMemberVotes = destination.GroupMemberVotes;

        // Assert
        groupMemberVotes.Should().NotBeNull();
        groupMemberVotes.Should().BeEmpty();
    }
}
