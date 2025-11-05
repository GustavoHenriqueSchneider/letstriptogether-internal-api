using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Migrations;

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
                columns: new[] { "Id", "UserId", "LikesCommercial", "Food", "Culture", 
                    "Entertainment", "PlaceTypes", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), userId, true, 
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) }, 
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Center) }, 
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Attraction) }, 
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Beach), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain) },
                        DateTime.UtcNow, null },

                    { Guid.NewGuid(), randomUser1, false,
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) },
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Center) },
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Sports) },
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Beach), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain) }, 
                        DateTime.UtcNow, null },

                    { Guid.NewGuid(), randomUser2, true,
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) },
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Center), new TripPreference.Culture(TripPreference.Culture.Historical) },
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Park), new TripPreference.Entertainment(TripPreference.Entertainment.Attraction) },
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Park), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain) },
                        DateTime.UtcNow, null },

                    { Guid.NewGuid(), randomUser3, false,
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) },
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Museum), new TripPreference.Culture(TripPreference.Culture.Center) },
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Sports) },
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Park), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain) },
                        DateTime.UtcNow, null },
                });
            
            var assembly = typeof(PasswordHashService).Assembly;
            var resourceName = "LetsTripTogether.InternalApi.Infrastructure.Dataset.cities_with_attractions.json";
            
            string jsonContent;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    // Listar recursos disponiveis para debug
                    var availableResources = assembly.GetManifestResourceNames();
                    throw new InvalidOperationException(
                        $"Embedded resource '{resourceName}' not found. " +
                        $"Available resources: {string.Join(", ", availableResources)}. " +
                        $"Make sure the file is marked as Embedded Resource in the .csproj file.");
                }
                
                using (var reader = new StreamReader(stream))
                {
                    jsonContent = reader.ReadToEnd();
                }
            }
            
            var jsonDoc = JsonDocument.Parse(jsonContent);
            
            var realDestinations = new List<(Guid Id, string Address, string Description)>();
            var destinationRows = new List<object[]>();
            var attractionRows = new List<object[]>();
            
            foreach (var cityElement in jsonDoc.RootElement.EnumerateArray())
            {
                var cityName = cityElement.GetProperty("name").GetString() ?? string.Empty;
                var cityDescription = cityElement.TryGetProperty("description", out var descElement) ? descElement.GetString() ?? string.Empty : string.Empty;
                
                var destinationId = Guid.NewGuid();
                realDestinations.Add((destinationId, cityName, cityDescription));
                
                destinationRows.Add(new object[]
                {
                    destinationId,
                    cityName,
                    cityDescription,
                    DateTime.UtcNow,
                    (DateTime?)null
                });
                
                // Processar atracoes
                if (cityElement.TryGetProperty("attractions", out var attractionsElement) && attractionsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var attractionElement in attractionsElement.EnumerateArray())
                    {
                        var attractionName = attractionElement.TryGetProperty("name", out var nameElem) ? nameElem.GetString() ?? string.Empty : string.Empty;
                        var attractionDesc = attractionElement.TryGetProperty("description", out var descElem) ? descElem.GetString() ?? string.Empty : string.Empty;
                        var category = attractionElement.TryGetProperty("category", out var catElem) ? catElem.GetString() ?? string.Empty : string.Empty;
                        
                        if (!string.IsNullOrEmpty(attractionName))
                        {
                            attractionRows.Add(new object[]
                            {
                                Guid.NewGuid(),
                                destinationId,
                                attractionName,
                                attractionDesc,
                                category,
                                DateTime.UtcNow,
                                (DateTime?)null
                            });
                        }
                    }
                }
            }
            
            if (destinationRows.Count > 0)
            {
                var valuesArray = new object[destinationRows.Count, 5];
                for (int i = 0; i < destinationRows.Count; i++)
                {
                    valuesArray[i, 0] = destinationRows[i][0]; // Id
                    valuesArray[i, 1] = destinationRows[i][1]; // Address
                    valuesArray[i, 2] = destinationRows[i][2]; // Description
                    valuesArray[i, 3] = destinationRows[i][3]; // CreatedAt
                    valuesArray[i, 4] = destinationRows[i][4]; // UpdatedAt
                }
                
                migrationBuilder.InsertData(
                    table: "Destinations",
                    columns: new[] { "Id", "Address", "Description", "CreatedAt", "UpdatedAt" },
                    values: valuesArray);
            }
            
            if (attractionRows.Count > 0)
            {
                var attractionValuesArray = new object[attractionRows.Count, 7];
                for (int i = 0; i < attractionRows.Count; i++)
                {
                    attractionValuesArray[i, 0] = attractionRows[i][0]; // Id
                    attractionValuesArray[i, 1] = attractionRows[i][1]; // DestinationId
                    attractionValuesArray[i, 2] = attractionRows[i][2]; // Name
                    attractionValuesArray[i, 3] = attractionRows[i][3]; // Description
                    attractionValuesArray[i, 4] = attractionRows[i][4]; // Category
                    attractionValuesArray[i, 5] = attractionRows[i][5]; // CreatedAt
                    attractionValuesArray[i, 6] = attractionRows[i][6]; // UpdatedAt
                }
                
                migrationBuilder.InsertData(
                    table: "DestinationAttractions",
                    columns: new[] { "Id", "DestinationId", "Name", "Description", "Category", "CreatedAt", "UpdatedAt" },
                    values: attractionValuesArray);
            }

            // setting group data
            var testGroupId = Guid.NewGuid();
            var testGroupId2 = Guid.NewGuid();
            var testGroupId3 = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name", "TripExpectedDate", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { testGroupId, "Test Group", DateTime.UtcNow.AddYears(3), DateTime.UtcNow, null },
                    { testGroupId2, "Test Group 2", DateTime.UtcNow.AddYears(1), DateTime.UtcNow, null },
                    { testGroupId3, "Test Group 3", DateTime.UtcNow.AddYears(2), DateTime.UtcNow, null }
                });

            var userMemberId = Guid.NewGuid();
            var exampleMemberId1 = Guid.NewGuid();
            var exampleMemberId2 = Guid.NewGuid();
            var exampleMemberId3 = Guid.NewGuid();
            var exampleMemberId4 = Guid.NewGuid();
            var exampleMemberId5 = Guid.NewGuid();
            var exampleMemberId6 = Guid.NewGuid();
            var exampleMemberId7 = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "GroupId", "UserId", "IsOwner", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { userMemberId, testGroupId, userId, true, DateTime.UtcNow, null },
                    { exampleMemberId1, testGroupId2, randomUser1, false, DateTime.UtcNow, null },
                    { exampleMemberId2, testGroupId2, randomUser2, true, DateTime.UtcNow, null },
                    { exampleMemberId3, testGroupId, randomUser3, false, DateTime.UtcNow, null },
                    { exampleMemberId4, testGroupId2, randomUser3, false, DateTime.UtcNow, null },
                    { exampleMemberId5, testGroupId3, userId, true, DateTime.UtcNow, null },
                    { exampleMemberId6, testGroupId3, randomUser1, false, DateTime.UtcNow, null },
                    { exampleMemberId7, testGroupId3, randomUser2, false, DateTime.UtcNow, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMemberDestinationVotes",
                columns: new[] { "Id", "GroupMemberId", "DestinationId", "IsApproved", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), userMemberId, realDestinations[1].Id, false, DateTime.UtcNow, null },
                    { Guid.NewGuid(), userMemberId, realDestinations[2].Id, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId1, realDestinations[1].Id, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId2, realDestinations[1].Id, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId3, realDestinations[2].Id, false, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId4, realDestinations[2].Id, true, DateTime.UtcNow, null },
                    { Guid.NewGuid(), exampleMemberId4, realDestinations[1].Id, true, DateTime.UtcNow, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMatches",
                columns: new[] { "Id", "GroupId", "DestinationId", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), testGroupId2, realDestinations[1].Id, DateTime.UtcNow, null }
                });

            migrationBuilder.InsertData(
                table: "GroupPreferences",
                columns: new[] { "Id", "GroupId", "LikesCommercial", "Food", "Culture",
                    "Entertainment", "PlaceTypes", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), testGroupId, true,
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) },
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Center), new TripPreference.Culture(TripPreference.Culture.Museum) },
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Attraction), new TripPreference.Entertainment(TripPreference.Entertainment.Sports) },
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Beach), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain), new TripPreference.PlaceType(TripPreference.PlaceType.Park) }, 
                        DateTime.UtcNow, null },

                    { Guid.NewGuid(), testGroupId2, true,
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) },
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Center), new TripPreference.Culture(TripPreference.Culture.Historical), new TripPreference.Culture(TripPreference.Culture.Museum) },
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Sports), new TripPreference.Entertainment(TripPreference.Entertainment.Park), new TripPreference.Entertainment(TripPreference.Entertainment.Attraction) },
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Beach), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain), new TripPreference.PlaceType(TripPreference.PlaceType.Park) },
                        DateTime.UtcNow, null },
                    
                    { Guid.NewGuid(), testGroupId3, true,
                        new List<string>{ new TripPreference.Food(TripPreference.Food.Restaurant) },
                        new List<string>{ new TripPreference.Culture(TripPreference.Culture.Center), new TripPreference.Culture(TripPreference.Culture.Historical) },
                        new List<string>{ new TripPreference.Entertainment(TripPreference.Entertainment.Attraction), new TripPreference.Entertainment(TripPreference.Entertainment.Sports), new TripPreference.Entertainment(TripPreference.Entertainment.Park) },
                        new List<string>{ new TripPreference.PlaceType(TripPreference.PlaceType.Beach), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain), new TripPreference.PlaceType(TripPreference.PlaceType.Park) }, 
                        DateTime.UtcNow, null }
                });
            
            // setting group invitations data
            var invitation1 = Guid.NewGuid();
            var invitation2 = Guid.NewGuid();
            var invitation3 = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "GroupInvitations",
                columns: new[] { "Id", "GroupId", "ExpirationDate", "Status", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { invitation1, testGroupId, DateTime.UtcNow - TimeSpan.FromDays(7), (int)GroupInvitationStatus.Expired, DateTime.UtcNow - TimeSpan.FromDays(14), DateTime.UtcNow - TimeSpan.FromDays(7) },
                    { invitation2, testGroupId2, DateTime.UtcNow.AddDays(7), (int)GroupInvitationStatus.Cancelled, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(10) },
                    { invitation3, testGroupId2, DateTime.UtcNow.AddDays(7), (int)GroupInvitationStatus.Active, DateTime.UtcNow, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
