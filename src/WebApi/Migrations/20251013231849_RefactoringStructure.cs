using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupInvitations_Groups_GroupId",
                table: "GroupInvitations");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "GroupInvitations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupInvitations_Groups_GroupId",
                table: "GroupInvitations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupInvitations_Groups_GroupId",
                table: "GroupInvitations");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "GroupInvitations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupInvitations_Groups_GroupId",
                table: "GroupInvitations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
