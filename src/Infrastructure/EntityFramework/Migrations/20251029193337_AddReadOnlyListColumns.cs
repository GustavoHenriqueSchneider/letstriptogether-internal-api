using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddReadOnlyListColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Categories",
                table: "Destinations",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "FoodPreferences",
                table: "UserPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "CulturePreferences",
                table: "UserPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "EntertainmentPreferences",
                table: "UserPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "PlaceTypes",
                table: "UserPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "FoodPreferences",
                table: "GroupPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "CulturePreferences",
                table: "GroupPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "EntertainmentPreferences",
                table: "GroupPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "PlaceTypes",
                table: "GroupPreferences",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "FoodPreferences",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "CulturePreferences",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "EntertainmentPreferences",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "PlaceTypes",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "FoodPreferences",
                table: "GroupPreferences");

            migrationBuilder.DropColumn(
                name: "CulturePreferences",
                table: "GroupPreferences");

            migrationBuilder.DropColumn(
                name: "EntertainmentPreferences",
                table: "GroupPreferences");

            migrationBuilder.DropColumn(
                name: "PlaceTypes",
                table: "GroupPreferences");
        }
    }
}
