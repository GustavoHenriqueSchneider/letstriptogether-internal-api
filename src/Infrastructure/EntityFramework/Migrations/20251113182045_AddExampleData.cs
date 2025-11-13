using System.IO;
using System.Text.Json;
using Domain.Aggregates.GroupAggregate.Enums;
using Domain.Security;
using Domain.ValueObjects.TripPreferences;
using Infrastructure.Services;
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
            var passwordHashService = new PasswordHashService();
            var now = DateTime.UtcNow;
            
            // ========== ROLES ==========
            var defaultRoleId = Guid.NewGuid();
            var adminRoleId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { defaultRoleId, Roles.User, now, now },
                    { adminRoleId, Roles.Admin, now, now }
                });

            // ========== USERS (10 users) ==========
            var userIds = new List<Guid>();
            for (int i = 0; i < 10; i++)
            {
                userIds.Add(Guid.NewGuid());
            }

            var userPassword = passwordHashService.HashPassword("User@123");
            var adminPassword = passwordHashService.HashPassword("Admin@123");

            var userNames = new[]
            {
                "Example User", "Admin", "Alice Silva", "Bob Santos", "Carol Costa",
                "David Oliveira", "Eva Pereira", "Fernando Lima", "Gabriela Alves", "Henrique Martins"
            };

            var userEmails = new[]
            {
                $"{Roles.User}@letstriptogether.com",
                $"{Roles.Admin}@letstriptogether.com",
                "alice.silva@letstriptogether.com",
                "bob.santos@letstriptogether.com",
                "carol.costa@letstriptogether.com",
                "david.oliveira@letstriptogether.com",
                "eva.pereira@letstriptogether.com",
                "fernando.lima@letstriptogether.com",
                "gabriela.alves@letstriptogether.com",
                "henrique.martins@letstriptogether.com"
            };

            var userPasswords = new[]
            {
                userPassword, adminPassword, userPassword, userPassword, userPassword,
                userPassword, userPassword, userPassword, userPassword, userPassword
            };

            var userValues = new object[10, 7];
            for (int i = 0; i < 10; i++)
            {
                userValues[i, 0] = userIds[i];
                userValues[i, 1] = userNames[i];
                userValues[i, 2] = userEmails[i];
                userValues[i, 3] = userPasswords[i];
                userValues[i, 4] = false;
                userValues[i, 5] = now;
                userValues[i, 6] = (DateTime?)null;
            }

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Email", "PasswordHash", "IsAnonymous", "CreatedAt", "UpdatedAt" },
                values: userValues);

            // ========== USER ROLES ==========
            var userRoleValues = new List<object[]>();
            for (int i = 0; i < 10; i++)
            {
                var roleId = i == 1 ? adminRoleId : defaultRoleId; // User 1 is admin
                userRoleValues.Add(new object[] { Guid.NewGuid(), userIds[i], roleId, now, now });
            }

            var userRoleArray = new object[userRoleValues.Count, 5];
            for (int i = 0; i < userRoleValues.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    userRoleArray[i, j] = userRoleValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "UserId", "RoleId", "CreatedAt", "UpdatedAt" },
                values: userRoleArray);

            // ========== USER PREFERENCES ==========
            var userPreferenceValues = new List<object[]>
            {
                new object[] { Guid.NewGuid(), userIds[0], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Center).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[1], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString(), new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Tour).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Park).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[2], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Center).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[3], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Center).ToString(), new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Park).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Park).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[4], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString(), new TripPreference.Culture(TripPreference.Culture.Center).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Park).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[5], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Adventure).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Trail).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[6], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Religious).ToString(), new TripPreference.Culture(TripPreference.Culture.Monument).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Viewpoint).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[7], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Architecture).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Tour).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Park).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Nature).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Waterfall).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[8], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Education).ToString(), new TripPreference.Culture(TripPreference.Culture.Museum).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), userIds[9], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Heritage).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Adventure).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Cave).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null }
            };

            var userPreferenceArray = new object[userPreferenceValues.Count, 9];
            for (int i = 0; i < userPreferenceValues.Count; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    userPreferenceArray[i, j] = userPreferenceValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "UserPreferences",
                columns: new[] { "Id", "UserId", "LikesShopping", "LikesGastronomy", "Culture", "Entertainment", "PlaceTypes", "CreatedAt", "UpdatedAt" },
                values: userPreferenceArray);

            // ========== DESTINATIONS (from JSON) ==========
            var assembly = typeof(PasswordHashService).Assembly;
            var resourceName = "Infrastructure.Dataset.cities_with_attractions.json";
            
            string jsonContent;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
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
                    now,
                    (DateTime?)null
                });
                
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
                                now,
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
                    valuesArray[i, 0] = destinationRows[i][0];
                    valuesArray[i, 1] = destinationRows[i][1];
                    valuesArray[i, 2] = destinationRows[i][2];
                    valuesArray[i, 3] = destinationRows[i][3];
                    valuesArray[i, 4] = destinationRows[i][4];
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
                    attractionValuesArray[i, 0] = attractionRows[i][0];
                    attractionValuesArray[i, 1] = attractionRows[i][1];
                    attractionValuesArray[i, 2] = attractionRows[i][2];
                    attractionValuesArray[i, 3] = attractionRows[i][3];
                    attractionValuesArray[i, 4] = attractionRows[i][4];
                    attractionValuesArray[i, 5] = attractionRows[i][5];
                    attractionValuesArray[i, 6] = attractionRows[i][6];
                }
                
                migrationBuilder.InsertData(
                    table: "DestinationAttractions",
                    columns: new[] { "Id", "DestinationId", "Name", "Description", "Category", "CreatedAt", "UpdatedAt" },
                    values: attractionValuesArray);
            }

            // ========== GROUPS (10 groups) ==========
            var groupIds = new List<Guid>();
            for (int i = 0; i < 10; i++)
            {
                groupIds.Add(Guid.NewGuid());
            }

            var groupNames = new[]
            {
                "Viagem para o Nordeste", "Aventura nas Montanhas", "Praias do Sul",
                "Tour Cultural SP", "Férias em Família", "Amigos do Trabalho",
                "Exploradores Urbanos", "Natureza e Aventura", "Roteiro Histórico",
                "Relax e Descanso"
            };

            var groupDates = new[]
            {
                now.AddMonths(6), now.AddMonths(3), now.AddMonths(9),
                now.AddMonths(4), now.AddMonths(12), now.AddMonths(2),
                now.AddMonths(5), now.AddMonths(8), now.AddMonths(7),
                now.AddMonths(10)
            };

            var groupValues = new object[10, 5];
            for (int i = 0; i < 10; i++)
            {
                groupValues[i, 0] = groupIds[i];
                groupValues[i, 1] = groupNames[i];
                groupValues[i, 2] = groupDates[i];
                groupValues[i, 3] = now;
                groupValues[i, 4] = (DateTime?)null;
            }

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name", "TripExpectedDate", "CreatedAt", "UpdatedAt" },
                values: groupValues);

            // ========== GROUP MEMBERS (varied combinations) ==========
            var groupMemberIds = new List<Guid>();
            var groupMemberValues = new List<object[]>();

            // Group 0: Users 0, 2, 3, 4 (User 0 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[0], userIds[0], true, now, null });
            for (int i = 2; i <= 4; i++)
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[0], userIds[i], false, now, null });
            }

            // Group 1: Users 1, 3, 5, 6 (User 1 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[1], userIds[1], true, now, null });
            foreach (var userId in new[] { userIds[3], userIds[5], userIds[6] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[1], userId, false, now, null });
            }

            // Group 2: Users 2, 4, 7 (User 2 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[2], userIds[2], true, now, null });
            foreach (var userId in new[] { userIds[4], userIds[7] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[2], userId, false, now, null });
            }

            // Group 3: Users 3, 5, 8, 9 (User 3 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[3], userIds[3], true, now, null });
            foreach (var userId in new[] { userIds[5], userIds[8], userIds[9] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[3], userId, false, now, null });
            }

            // Group 4: Users 4, 6, 7 (User 4 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[4], userIds[4], true, now, null });
            foreach (var userId in new[] { userIds[6], userIds[7] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[4], userId, false, now, null });
            }

            // Group 5: Users 5, 7, 8 (User 5 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[5], userIds[5], true, now, null });
            foreach (var userId in new[] { userIds[7], userIds[8] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[5], userId, false, now, null });
            }

            // Group 6: Users 0, 6, 9 (User 0 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[6], userIds[0], true, now, null });
            foreach (var userId in new[] { userIds[6], userIds[9] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[6], userId, false, now, null });
            }

            // Group 7: Users 1, 2, 8 (User 1 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[7], userIds[1], true, now, null });
            foreach (var userId in new[] { userIds[2], userIds[8] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[7], userId, false, now, null });
            }

            // Group 8: Users 2, 3, 5, 6 (User 2 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[8], userIds[2], true, now, null });
            foreach (var userId in new[] { userIds[3], userIds[5], userIds[6] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[8], userId, false, now, null });
            }

            // Group 9: Users 0, 1, 4, 7, 9 (User 0 is owner)
            groupMemberIds.Add(Guid.NewGuid());
            groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[9], userIds[0], true, now, null });
            foreach (var userId in new[] { userIds[1], userIds[4], userIds[7], userIds[9] })
            {
                groupMemberIds.Add(Guid.NewGuid());
                groupMemberValues.Add(new object[] { groupMemberIds[^1], groupIds[9], userId, false, now, null });
            }

            var groupMemberArray = new object[groupMemberValues.Count, 6];
            for (int i = 0; i < groupMemberValues.Count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    groupMemberArray[i, j] = groupMemberValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "GroupId", "UserId", "IsOwner", "CreatedAt", "UpdatedAt" },
                values: groupMemberArray);

            // ========== GROUP MEMBER DESTINATION VOTES ==========
            var voteValues = new List<object[]>();
            
            void AddVotesForMember(Guid memberId, int startDestIndex, bool[] approvals)
            {
                for (int i = 0; i < approvals.Length && (startDestIndex + i) < realDestinations.Count; i++)
                {
                    voteValues.Add(new object[] { Guid.NewGuid(), memberId, realDestinations[startDestIndex + i].Id, approvals[i], now, null });
                }
            }

            if (realDestinations.Count >= 8)
            {
                AddVotesForMember(groupMemberIds[0], 0, new[] { true, false, true, true });  // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[1], 0, new[] { true, true, true, false }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[2], 0, new[] { true, true, false, false }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[3], 0, new[] { true, false, true, true }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[0], 4, new[] { true, false, true, false }); // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[1], 4, new[] { true, true, false, true });  // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[2], 4, new[] { true, false, true, true }); // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[3], 4, new[] { true, true, true, false });  // Dest 4,5,6,7
                
                var g1Start = 4;
                AddVotesForMember(groupMemberIds[g1Start], 1, new[] { true, false, true, true });     // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g1Start + 1], 1, new[] { true, true, false, true }); // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g1Start + 2], 1, new[] { true, true, true, false }); // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g1Start + 3], 1, new[] { true, false, true, true }); // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g1Start], 3, new[] { true, false, true, false });     // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g1Start + 1], 3, new[] { true, true, false, true }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g1Start + 2], 3, new[] { true, true, true, false }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g1Start + 3], 3, new[] { true, false, true, true }); // Dest 3,4,5,6
                
                var g2Start = 8;
                AddVotesForMember(groupMemberIds[g2Start], 2, new[] { true, false, true, true });     // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g2Start + 1], 2, new[] { true, true, false, true }); // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g2Start + 2], 2, new[] { true, true, true, false }); // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g2Start], 5, new[] { true, false, true, false });     // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g2Start + 1], 5, new[] { true, true, false, true }); // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g2Start + 2], 5, new[] { true, true, true, false }); // Dest 5,6,7,0
                
                var g3Start = 11;
                AddVotesForMember(groupMemberIds[g3Start], 3, new[] { true, false, true, true });     // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g3Start + 1], 3, new[] { true, true, false, true }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g3Start + 2], 3, new[] { true, true, true, false }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g3Start + 3], 3, new[] { true, false, true, true }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g3Start], 6, new[] { true, false, true, false });     // Dest 6,7,0,1
                AddVotesForMember(groupMemberIds[g3Start + 1], 6, new[] { true, true, false, true }); // Dest 6,7,0,1
                AddVotesForMember(groupMemberIds[g3Start + 2], 6, new[] { true, true, true, false }); // Dest 6,7,0,1
                AddVotesForMember(groupMemberIds[g3Start + 3], 6, new[] { true, false, true, true }); // Dest 6,7,0,1
                
                var g4Start = 15;
                AddVotesForMember(groupMemberIds[g4Start], 4, new[] { true, false, true, true });     // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[g4Start + 1], 4, new[] { true, true, false, true }); // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[g4Start + 2], 4, new[] { true, true, true, false }); // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[g4Start], 7, new[] { true, false, true, false });     // Dest 7,0,1,2
                AddVotesForMember(groupMemberIds[g4Start + 1], 7, new[] { true, true, false, true }); // Dest 7,0,1,2
                AddVotesForMember(groupMemberIds[g4Start + 2], 7, new[] { true, true, true, false }); // Dest 7,0,1,2
                
                var g5Start = 18;
                AddVotesForMember(groupMemberIds[g5Start], 5, new[] { true, false, true, true });     // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g5Start + 1], 5, new[] { true, true, false, true }); // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g5Start + 2], 5, new[] { true, true, true, false }); // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g5Start], 0, new[] { true, false, true, false });     // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g5Start + 1], 0, new[] { true, true, false, true }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g5Start + 2], 0, new[] { true, true, true, false }); // Dest 0,1,2,3
                
                var g6Start = 21;
                AddVotesForMember(groupMemberIds[g6Start], 0, new[] { true, false, true, true });     // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g6Start + 1], 0, new[] { true, true, false, true }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g6Start + 2], 0, new[] { true, true, true, false }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g6Start], 1, new[] { true, false, true, false });     // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g6Start + 1], 1, new[] { true, true, false, true }); // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g6Start + 2], 1, new[] { true, true, true, false }); // Dest 1,2,3,4
                
                var g7Start = 24;
                AddVotesForMember(groupMemberIds[g7Start], 1, new[] { true, false, true, true });     // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g7Start + 1], 1, new[] { true, true, false, true }); // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g7Start + 2], 1, new[] { true, true, true, false }); // Dest 1,2,3,4
                AddVotesForMember(groupMemberIds[g7Start], 4, new[] { true, false, true, false });     // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[g7Start + 1], 4, new[] { true, true, false, true }); // Dest 4,5,6,7
                AddVotesForMember(groupMemberIds[g7Start + 2], 4, new[] { true, true, true, false }); // Dest 4,5,6,7
                
                var g8Start = 27;
                AddVotesForMember(groupMemberIds[g8Start], 2, new[] { true, false, true, true });     // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g8Start + 1], 2, new[] { true, true, false, true }); // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g8Start + 2], 2, new[] { true, true, true, false }); // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g8Start + 3], 2, new[] { true, false, true, true }); // Dest 2,3,4,5
                AddVotesForMember(groupMemberIds[g8Start], 5, new[] { true, false, true, false });     // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g8Start + 1], 5, new[] { true, true, false, true }); // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g8Start + 2], 5, new[] { true, true, true, false }); // Dest 5,6,7,0
                AddVotesForMember(groupMemberIds[g8Start + 3], 5, new[] { true, false, true, true }); // Dest 5,6,7,0
                
                var g9Start = 31;
                AddVotesForMember(groupMemberIds[g9Start], 0, new[] { true, false, true, true });     // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g9Start + 1], 0, new[] { true, true, false, true }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g9Start + 2], 0, new[] { true, true, true, false }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g9Start + 3], 0, new[] { true, false, true, true }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g9Start + 4], 0, new[] { true, true, false, true }); // Dest 0,1,2,3
                AddVotesForMember(groupMemberIds[g9Start], 3, new[] { true, false, true, false });     // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g9Start + 1], 3, new[] { true, true, false, true }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g9Start + 2], 3, new[] { true, true, true, false }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g9Start + 3], 3, new[] { true, false, true, true }); // Dest 3,4,5,6
                AddVotesForMember(groupMemberIds[g9Start + 4], 3, new[] { true, true, false, true }); // Dest 3,4,5,6
            }

            var voteArray = new object[voteValues.Count, 6];
            for (int i = 0; i < voteValues.Count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    voteArray[i, j] = voteValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "GroupMemberDestinationVotes",
                columns: new[] { "Id", "GroupMemberId", "DestinationId", "IsApproved", "CreatedAt", "UpdatedAt" },
                values: voteArray);

            // ========== GROUP MATCHES (when all members approved) ==========
            var matchValues = new List<object[]>();
            
            if (realDestinations.Count >= 8)
            {
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[0], realDestinations[0].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[0], realDestinations[4].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[1], realDestinations[1].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[1], realDestinations[3].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[2], realDestinations[2].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[2], realDestinations[5].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[3], realDestinations[3].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[3], realDestinations[6].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[4], realDestinations[4].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[4], realDestinations[7].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[5], realDestinations[5].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[5], realDestinations[0].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[6], realDestinations[0].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[6], realDestinations[1].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[7], realDestinations[1].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[7], realDestinations[4].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[8], realDestinations[2].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[8], realDestinations[5].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[9], realDestinations[0].Id, now, null });
                matchValues.Add(new object[] { Guid.NewGuid(), groupIds[9], realDestinations[3].Id, now, null });
            }

            if (matchValues.Count > 0)
            {
                var matchArray = new object[matchValues.Count, 5];
                for (int i = 0; i < matchValues.Count; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        matchArray[i, j] = matchValues[i][j];
                    }
                }

                migrationBuilder.InsertData(
                    table: "GroupMatches",
                    columns: new[] { "Id", "GroupId", "DestinationId", "CreatedAt", "UpdatedAt" },
                    values: matchArray);
            }

            // ========== GROUP PREFERENCES ==========
            var groupPreferenceValues = new List<object[]>
            {
                new object[] { Guid.NewGuid(), groupIds[0], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Center).ToString(), new TripPreference.Culture(TripPreference.Culture.Museum).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Park).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[1], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString(), new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Tour).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Trail).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[2], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Center).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[3], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Center).ToString(), new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Park).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Park).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[4], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString(), new TripPreference.Culture(TripPreference.Culture.Center).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Park).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[5], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Adventure).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Trail).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[6], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Religious).ToString(), new TripPreference.Culture(TripPreference.Culture.Monument).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Viewpoint).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[7], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Architecture).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Tour).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Park).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Nature).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Waterfall).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[8], true, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Education).ToString(), new TripPreference.Culture(TripPreference.Culture.Museum).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString() },
                    now, null },
                new object[] { Guid.NewGuid(), groupIds[9], false, true,
                    new List<string> { new TripPreference.Culture(TripPreference.Culture.Heritage).ToString() },
                    new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Sports).ToString(), new TripPreference.Entertainment(TripPreference.Entertainment.Adventure).ToString() },
                    new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Cave).ToString(), new TripPreference.PlaceType(TripPreference.PlaceType.Mountain).ToString() },
                    now, null }
            };

            var groupPreferenceArray = new object[groupPreferenceValues.Count, 9];
            for (int i = 0; i < groupPreferenceValues.Count; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    groupPreferenceArray[i, j] = groupPreferenceValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "GroupPreferences",
                columns: new[] { "Id", "GroupId", "LikesShopping", "LikesGastronomy", "Culture", "Entertainment", "PlaceTypes", "CreatedAt", "UpdatedAt" },
                values: groupPreferenceArray);

            // ========== GROUP INVITATIONS (with different statuses) ==========
            var invitationIds = new List<Guid>();
            var invitationValues = new List<object[]>();
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[0], now - TimeSpan.FromDays(7), (int)GroupInvitationStatus.Expired, now - TimeSpan.FromDays(14), now - TimeSpan.FromDays(7) });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[2], now - TimeSpan.FromDays(3), (int)GroupInvitationStatus.Expired, now - TimeSpan.FromDays(10), now - TimeSpan.FromDays(3) });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[5], now - TimeSpan.FromDays(1), (int)GroupInvitationStatus.Expired, now - TimeSpan.FromDays(8), now - TimeSpan.FromDays(1) });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[1], now.AddDays(7), (int)GroupInvitationStatus.Cancelled, now, now.AddSeconds(10) });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[3], now.AddDays(5), (int)GroupInvitationStatus.Cancelled, now.AddDays(-2), now.AddDays(-1) });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[7], now.AddDays(10), (int)GroupInvitationStatus.Cancelled, now.AddDays(-1), now.AddHours(2) });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[4], now.AddDays(7), (int)GroupInvitationStatus.Active, now, null });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[6], now.AddDays(14), (int)GroupInvitationStatus.Active, now.AddDays(-1), null });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[8], now.AddDays(3), (int)GroupInvitationStatus.Active, now.AddHours(-5), null });
            
            invitationIds.Add(Guid.NewGuid());
            invitationValues.Add(new object[] { invitationIds[^1], groupIds[9], now.AddDays(21), (int)GroupInvitationStatus.Active, now.AddDays(-3), null });

            var invitationArray = new object[invitationValues.Count, 6];
            for (int i = 0; i < invitationValues.Count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    invitationArray[i, j] = invitationValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "GroupInvitations",
                columns: new[] { "Id", "GroupId", "ExpirationDate", "Status", "CreatedAt", "UpdatedAt" },
                values: invitationArray);

            // ========== USER GROUP INVITATIONS (answers to invitations) ==========
            var userGroupInvitationValues = new List<object[]>();
            
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[0], userIds[5], true, now - TimeSpan.FromDays(10), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[0], userIds[6], false, now - TimeSpan.FromDays(9), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[1], userIds[7], true, now - TimeSpan.FromDays(5), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[3], userIds[8], true, now.AddDays(-1), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[3], userIds[9], false, now.AddDays(-1).AddHours(2), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[6], userIds[0], true, now.AddHours(-2), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[6], userIds[1], false, now.AddHours(-1), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[7], userIds[2], true, now.AddDays(-1).AddHours(3), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[7], userIds[3], true, now.AddDays(-1).AddHours(4), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[8], userIds[4], false, now.AddHours(-4), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[9], userIds[5], true, now.AddDays(-2).AddHours(1), null });
            userGroupInvitationValues.Add(new object[] { Guid.NewGuid(), invitationIds[9], userIds[6], true, now.AddDays(-2).AddHours(2), null });

            var userGroupInvitationArray = new object[userGroupInvitationValues.Count, 6];
            for (int i = 0; i < userGroupInvitationValues.Count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    userGroupInvitationArray[i, j] = userGroupInvitationValues[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "UserGroupInvitations",
                columns: new[] { "Id", "GroupInvitationId", "UserId", "IsAccepted", "CreatedAt", "UpdatedAt" },
                values: userGroupInvitationArray);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        
        }
    }
}
