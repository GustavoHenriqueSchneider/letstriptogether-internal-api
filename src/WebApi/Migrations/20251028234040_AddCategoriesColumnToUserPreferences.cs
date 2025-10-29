using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesColumnToUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Categories",
                table: "UserPreferences",
                type: "text[]",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"UserPreferences\" SET \"Categories\" = ARRAY[]::text[] WHERE \"Categories\" IS NULL");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Categories",
                table: "UserPreferences",
                type: "text[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "UserPreferences");
        }
    }
}
