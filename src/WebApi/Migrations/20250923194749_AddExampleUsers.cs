using Microsoft.EntityFrameworkCore.Migrations;
using WebApi.Security;
using WebApi.Services;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddExampleUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            var userPassword = passwordHashService.HashPassword("user@123");
            var adminPassword = passwordHashService.HashPassword("admin@123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Email", "PasswordHash", "IsAnonymous", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { userId, "Example User", $"{Roles.User}@letstriptogether.com", userPassword, false, DateTime.UtcNow, DateTime.UtcNow },
                    { adminId, "Admin", $"{Roles.Admin}@letstriptogether.com", adminPassword, false, DateTime.UtcNow, DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "UserId", "RoleId", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), userId, defaultRoleId, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid(), adminId, adminRoleId, DateTime.UtcNow, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
