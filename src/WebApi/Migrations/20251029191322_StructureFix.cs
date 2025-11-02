using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class StructureFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPreference_Groups_GroupId",
                table: "GroupPreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupPreference",
                table: "GroupPreference");

            migrationBuilder.RenameTable(
                name: "GroupPreference",
                newName: "GroupPreferences");

            migrationBuilder.RenameIndex(
                name: "IX_GroupPreference_GroupId",
                table: "GroupPreferences",
                newName: "IX_GroupPreferences_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupPreferences",
                table: "GroupPreferences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPreferences_Groups_GroupId",
                table: "GroupPreferences",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPreferences_Groups_GroupId",
                table: "GroupPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupPreferences",
                table: "GroupPreferences");

            migrationBuilder.RenameTable(
                name: "GroupPreferences",
                newName: "GroupPreference");

            migrationBuilder.RenameIndex(
                name: "IX_GroupPreferences_GroupId",
                table: "GroupPreference",
                newName: "IX_GroupPreference_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupPreference",
                table: "GroupPreference",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPreference_Groups_GroupId",
                table: "GroupPreference",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
