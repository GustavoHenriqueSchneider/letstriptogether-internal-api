using Microsoft.EntityFrameworkCore.Migrations;
using WebApi.Models.ValueObjects;
using WebApi.Security;
using WebApi.Services.Implementations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddExampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // setting roles data
            var passwordHashService = new PasswordHashService();
            var defaultRoleId = Guid.NewGuid();
            var adminRoleId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { defaultRoleId, Roles.User, DateTime.UtcNow, DateTime.UtcNow },
                    { adminRoleId, Roles.Admin, DateTime.UtcNow, DateTime.UtcNow }
                });

            // settings users data
            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            var randomUser1 = Guid.NewGuid();
            var randomUser2 = Guid.NewGuid();
            var randomUser3 = Guid.NewGuid();

            var userPassword = passwordHashService.HashPassword("user@123");
            var adminPassword = passwordHashService.HashPassword("admin@123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Email", "PasswordHash", "IsAnonymous", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { userId, "Example User", $"{Roles.User}@letstriptogether.com", userPassword, false, DateTime.UtcNow, DateTime.UtcNow },
                    { adminId, "Admin", $"{Roles.Admin}@letstriptogether.com", adminPassword, false, DateTime.UtcNow, DateTime.UtcNow },
                    { randomUser1, "User 1", $"{Roles.User}1@letstriptogether.com", userPassword, false, DateTime.UtcNow, DateTime.UtcNow },
                    { randomUser2, "User 2", $"{Roles.User}2@letstriptogether.com", userPassword, false, DateTime.UtcNow, DateTime.UtcNow },
                    { randomUser3, "User 3", $"{Roles.User}3@letstriptogether.com", userPassword, false, DateTime.UtcNow, DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "UserId", "RoleId", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), userId, defaultRoleId, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid(), adminId, adminRoleId, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid(), randomUser1, defaultRoleId, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid(), randomUser2, defaultRoleId, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid(), randomUser3, defaultRoleId, DateTime.UtcNow, DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "UserPreferences",
                columns: new[] { "Id", "UserId", "LikesCommercial", "FoodPreferences", "CulturePreferences", 
                    "EntertainmentPreferences", "PlaceTypes", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), userId, true, 
                        new List<string>{ new TripFoodPreferences(nameof(TripFoodPreferences.FastFood)), new TripFoodPreferences(nameof(TripFoodPreferences.Bar)) }, 
                        new List<string>{ new TripCulturePreferences(nameof(TripCulturePreferences.Culture)), new TripCulturePreferences(nameof(TripCulturePreferences.Attraction)) }, 
                        new List<string>{ new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Cinema)) }, 
                        new List<string>{ new TripPlaceTypes(nameof(TripPlaceTypes.Beach)), new TripPlaceTypes(nameof(TripPlaceTypes.Mountain)) }, DateTime.UtcNow, null },

                    { Guid.NewGuid(), randomUser1, false,
                        new List<string>{ new TripFoodPreferences(nameof(TripFoodPreferences.Bar)) },
                        new List<string>{ new TripCulturePreferences(nameof(TripCulturePreferences.Culture)) },
                        new List<string>{ new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Sport)) },
                        new List<string>{ new TripPlaceTypes(nameof(TripPlaceTypes.Beach)), new TripPlaceTypes(nameof(TripPlaceTypes.Mountain)) }, DateTime.UtcNow, null },

                    { Guid.NewGuid(), randomUser2, true,
                        new List<string>{ new TripFoodPreferences(nameof(TripFoodPreferences.FastFood)), new TripFoodPreferences(nameof(TripFoodPreferences.Bar)) },
                        new List<string>{ new TripCulturePreferences(nameof(TripCulturePreferences.Culture)), new TripCulturePreferences(nameof(TripCulturePreferences.Sights)) },
                        new List<string>{ new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Spa)), new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Cinema)) },
                        new List<string>{ new TripPlaceTypes(nameof(TripPlaceTypes.Park)), new TripPlaceTypes(nameof(TripPlaceTypes.Mountain)) }, DateTime.UtcNow, null },

                    { Guid.NewGuid(), randomUser3, false,
                        new List<string>{ new TripFoodPreferences(nameof(TripFoodPreferences.Restaurant)) },
                        new List<string>{ new TripCulturePreferences(nameof(TripCulturePreferences.Museum)), new TripCulturePreferences(nameof(TripCulturePreferences.Attraction)) },
                        new List<string>{ new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Sport)) },
                        new List<string>{ new TripPlaceTypes(nameof(TripPlaceTypes.Park)), new TripPlaceTypes(nameof(TripPlaceTypes.Mountain)) }, DateTime.UtcNow, null },
                });

            // setting destinations
            var destination1 = Guid.NewGuid();
            var destination2 = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Destinations",
                columns: new[] { "Id", "Address", "Categories", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { destination1, "Rua avenida oi", new List<string> { "tourism", "funny" }, DateTime.UtcNow, null },
                    { destination2, "Rua teste 123", new List<string> { "warm", "cultural" }, DateTime.UtcNow, null }
                });

            // setting group data
            var testGroupId = Guid.NewGuid();
            var testGroupId2 = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name", "TripExpectedDate", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { testGroupId, "Test Group", DateTime.UtcNow.AddYears(3), DateTime.UtcNow, null },
                    { testGroupId2, "Test Group 2", DateTime.UtcNow.AddYears(1), DateTime.UtcNow, null }
                });

            var userMemberId = Guid.NewGuid();
            var adminMemberId = Guid.NewGuid();
            var exampleMemberId1 = Guid.NewGuid();
            var exampleMemberId2 = Guid.NewGuid();
            var exampleMemberId3 = Guid.NewGuid();
            var exampleMemberId4 = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "GroupId", "UserId", "IsOwner", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { adminMemberId, testGroupId, adminId, true, DateTime.UtcNow, null },
                    { userMemberId, testGroupId, userId, false, DateTime.UtcNow, null },
                    { exampleMemberId1, testGroupId2, randomUser1, false, DateTime.UtcNow, null },
                    { exampleMemberId2, testGroupId2, randomUser2, true, DateTime.UtcNow, null },
                    { exampleMemberId3, testGroupId, randomUser3, false, DateTime.UtcNow, null },
                    { exampleMemberId4, testGroupId2, randomUser3, false, DateTime.UtcNow, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMemberDestinationVotes",
                columns: new[] { "Id", "GroupMemberId", "DestinationId", "IsApproved", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), adminMemberId, destination1, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), adminMemberId, destination2, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), userMemberId, destination1, false, DateTime.UtcNow, null },
                    { Guid.NewGuid(), userMemberId, destination2, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId1, destination1, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId2, destination1, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId3, destination2, false, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId4, destination2, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId4, destination1, true, DateTime.UtcNow, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMatches",
                columns: new[] { "Id", "GroupId", "DestinationId", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), testGroupId2, destination1, DateTime.UtcNow, null }
                });

            migrationBuilder.InsertData(
                table: "GroupPreferences",
                columns: new[] { "Id", "GroupId", "LikesCommercial", "FoodPreferences", "CulturePreferences",
                    "EntertainmentPreferences", "PlaceTypes", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), testGroupId, true,
                        new List<string>{ new TripFoodPreferences(nameof(TripFoodPreferences.FastFood)), new TripFoodPreferences(nameof(TripFoodPreferences.Bar)) },
                        new List<string>{ new TripCulturePreferences(nameof(TripCulturePreferences.Culture)), new TripCulturePreferences(nameof(TripCulturePreferences.Attraction)) },
                        new List<string>{ new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Cinema)) },
                        new List<string>{ new TripPlaceTypes(nameof(TripPlaceTypes.Beach)), new TripPlaceTypes(nameof(TripPlaceTypes.Mountain)) }, DateTime.UtcNow, null },

                    { Guid.NewGuid(), testGroupId2, true,
                        new List<string>{ new TripFoodPreferences(nameof(TripFoodPreferences.Bar)), new TripFoodPreferences(nameof(TripFoodPreferences.FastFood)), new TripFoodPreferences(nameof(TripFoodPreferences.Restaurant)) },
                        new List<string>{ new TripCulturePreferences(nameof(TripCulturePreferences.Culture)), new TripCulturePreferences(nameof(TripCulturePreferences.Sights)), new TripCulturePreferences(nameof(TripCulturePreferences.Museum)), new TripCulturePreferences(nameof(TripCulturePreferences.Attraction)) },
                        new List<string>{ new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Sport)), new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Spa)), new TripEntertainmentPreferences(nameof(TripEntertainmentPreferences.Cinema)) },
                        new List<string>{ new TripPlaceTypes(nameof(TripPlaceTypes.Beach)), new TripPlaceTypes(nameof(TripPlaceTypes.Mountain)), new TripPlaceTypes(nameof(TripPlaceTypes.Park)) }, DateTime.UtcNow, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
