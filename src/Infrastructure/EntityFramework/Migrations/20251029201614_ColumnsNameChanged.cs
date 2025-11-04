using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ColumnsNameChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FoodPreferences",
                table: "UserPreferences",
                newName: "Food");

            migrationBuilder.RenameColumn(
                name: "EntertainmentPreferences",
                table: "UserPreferences",
                newName: "Entertainment");

            migrationBuilder.RenameColumn(
                name: "CulturePreferences",
                table: "UserPreferences",
                newName: "Culture");

            migrationBuilder.RenameColumn(
                name: "FoodPreferences",
                table: "GroupPreferences",
                newName: "Food");

            migrationBuilder.RenameColumn(
                name: "EntertainmentPreferences",
                table: "GroupPreferences",
                newName: "Entertainment");

            migrationBuilder.RenameColumn(
                name: "CulturePreferences",
                table: "GroupPreferences",
                newName: "Culture");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Food",
                table: "UserPreferences",
                newName: "FoodPreferences");

            migrationBuilder.RenameColumn(
                name: "Entertainment",
                table: "UserPreferences",
                newName: "EntertainmentPreferences");

            migrationBuilder.RenameColumn(
                name: "Culture",
                table: "UserPreferences",
                newName: "CulturePreferences");

            migrationBuilder.RenameColumn(
                name: "Food",
                table: "GroupPreferences",
                newName: "FoodPreferences");

            migrationBuilder.RenameColumn(
                name: "Entertainment",
                table: "GroupPreferences",
                newName: "EntertainmentPreferences");

            migrationBuilder.RenameColumn(
                name: "Culture",
                table: "GroupPreferences",
                newName: "CulturePreferences");
        }
    }
}
