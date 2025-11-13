using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixingFoodPreferenceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Food",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Food",
                table: "GroupPreferences");

            migrationBuilder.RenameColumn(
                name: "LikesCommercial",
                table: "UserPreferences",
                newName: "LikesShopping");

            migrationBuilder.RenameColumn(
                name: "LikesCommercial",
                table: "GroupPreferences",
                newName: "LikesShopping");

            migrationBuilder.AddColumn<bool>(
                name: "LikesGastronomy",
                table: "UserPreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LikesGastronomy",
                table: "GroupPreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikesGastronomy",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "LikesGastronomy",
                table: "GroupPreferences");

            migrationBuilder.RenameColumn(
                name: "LikesShopping",
                table: "UserPreferences",
                newName: "LikesCommercial");

            migrationBuilder.RenameColumn(
                name: "LikesShopping",
                table: "GroupPreferences",
                newName: "LikesCommercial");

            migrationBuilder.AddColumn<List<string>>(
                name: "Food",
                table: "UserPreferences",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "Food",
                table: "GroupPreferences",
                type: "text[]",
                nullable: false);
        }
    }
}
