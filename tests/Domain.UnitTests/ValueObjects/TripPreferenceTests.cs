using FluentAssertions;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using NUnit.Framework;

namespace Domain.UnitTests.ValueObjects;

[TestFixture]
public class TripPreferenceTests
{
    [TestFixture]
    public class TripPreferenceToStringTests
    {
        [Test]
        public void ToString_WithShoppingPreference_ShouldReturnShopping()
        {
            // Arrange
            var preference = new TripPreference(TripPreference.Shopping);

            // Act
            var result = preference.ToString();

            // Assert
            result.Should().Be(TripPreference.Shopping);
        }

        [Test]
        public void ToString_WithCulturePreference_ShouldReturnLowercaseCategory()
        {
            // Arrange
            var preference = new TripPreference(TripPreference.Culture.Museum);

            // Act
            var result = preference.ToString();

            // Assert
            result.Should().Be(nameof(TripPreference.Culture.Museum).ToLower());
        }

        [Test]
        public void ToString_WithEntertainmentPreference_ShouldReturnLowercaseCategory()
        {
            // Arrange
            var preference = new TripPreference(TripPreference.Entertainment.Attraction);

            // Act
            var result = preference.ToString();

            // Assert
            result.Should().Be(nameof(TripPreference.Entertainment.Attraction).ToLower());
        }

        [Test]
        public void ToString_WithFoodPreference_ShouldReturnLowercaseCategory()
        {
            // Arrange
            var preference = new TripPreference(TripPreference.Food.Restaurant);

            // Act
            var result = preference.ToString();

            // Assert
            result.Should().Be(nameof(TripPreference.Food.Restaurant).ToLower());
        }

        [Test]
        public void ToString_WithPlaceTypePreference_ShouldReturnLowercaseCategory()
        {
            // Arrange
            var preference = new TripPreference(TripPreference.PlaceType.Beach);

            // Act
            var result = preference.ToString();

            // Assert
            result.Should().Be(nameof(TripPreference.PlaceType.Beach).ToLower());
        }
    }

    [TestFixture]
    public class CultureToStringTests
    {
        [Test]
        public void ToString_WithArchitecture_ShouldReturnLowercaseKey()
        {
            // Arrange
            var culture = new TripPreference.Culture(TripPreference.Culture.Architecture);

            // Act
            var result = culture.ToString();

            // Assert
            result.Should().Be("architecture");
        }

        [Test]
        public void ToString_WithMuseum_ShouldReturnLowercaseKey()
        {
            // Arrange
            var culture = new TripPreference.Culture(TripPreference.Culture.Museum);

            // Act
            var result = culture.ToString();

            // Assert
            result.Should().Be("museum");
        }

        [Test]
        public void ToString_WithReligious_ShouldReturnLowercaseKey()
        {
            // Arrange
            var culture = new TripPreference.Culture(TripPreference.Culture.Religious);

            // Act
            var result = culture.ToString();

            // Assert
            result.Should().Be("religious");
        }
    }

    [TestFixture]
    public class EntertainmentToStringTests
    {
        [Test]
        public void ToString_WithAdventure_ShouldReturnLowercaseKey()
        {
            // Arrange
            var entertainment = new TripPreference.Entertainment(TripPreference.Entertainment.Adventure);

            // Act
            var result = entertainment.ToString();

            // Assert
            result.Should().Be("adventure");
        }

        [Test]
        public void ToString_WithAttraction_ShouldReturnLowercaseKey()
        {
            // Arrange
            var entertainment = new TripPreference.Entertainment(TripPreference.Entertainment.Attraction);

            // Act
            var result = entertainment.ToString();

            // Assert
            result.Should().Be("attraction");
        }

        [Test]
        public void ToString_WithPark_ShouldReturnLowercaseKey()
        {
            // Arrange
            var entertainment = new TripPreference.Entertainment(TripPreference.Entertainment.Park);

            // Act
            var result = entertainment.ToString();

            // Assert
            result.Should().Be("park");
        }
    }

    [TestFixture]
    public class FoodToStringTests
    {
        [Test]
        public void ToString_WithRestaurant_ShouldReturnLowercaseKey()
        {
            // Arrange
            var food = new TripPreference.Food(TripPreference.Food.Restaurant);

            // Act
            var result = food.ToString();

            // Assert
            result.Should().Be("restaurant");
        }
    }

    [TestFixture]
    public class PlaceTypeToStringTests
    {
        [Test]
        public void ToString_WithBeach_ShouldReturnLowercaseKey()
        {
            // Arrange
            var placeType = new TripPreference.PlaceType(TripPreference.PlaceType.Beach);

            // Act
            var result = placeType.ToString();

            // Assert
            result.Should().Be("beach");
        }

        [Test]
        public void ToString_WithMountain_ShouldReturnLowercaseKey()
        {
            // Arrange
            var placeType = new TripPreference.PlaceType(TripPreference.PlaceType.Mountain);

            // Act
            var result = placeType.ToString();

            // Assert
            result.Should().Be("mountain");
        }

        [Test]
        public void ToString_WithWaterfall_ShouldReturnLowercaseKey()
        {
            // Arrange
            var placeType = new TripPreference.PlaceType(TripPreference.PlaceType.Waterfall);

            // Act
            var result = placeType.ToString();

            // Assert
            result.Should().Be("waterfall");
        }
    }
}
