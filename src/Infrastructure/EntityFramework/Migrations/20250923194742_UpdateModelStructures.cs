using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelStructures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupInvitations_Groups_GroupId",
                table: "GroupInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMatches_Destinations_DestinationId",
                table: "GroupMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMatches_Groups_GroupId",
                table: "GroupMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberDestinationVotes_Destinations_DestinationId",
                table: "GroupMemberDestinationVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberDestinationVotes_GroupMembers_GroupMemberId",
                table: "GroupMemberDestinationVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Groups_GroupId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupInvitations_GroupInvitations_GroupInvitationId",
                table: "UserGroupInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupInvitations_Users_UserId",
                table: "UserGroupInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferences",
                table: "UserPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroupInvitations",
                table: "UserGroupInvitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMemberDestinationVotes",
                table: "GroupMemberDestinationVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMatches",
                table: "GroupMatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupInvitations",
                table: "GroupInvitations");

            migrationBuilder.RenameTable(
                name: "UserPreferences",
                newName: "UserPreference");

            migrationBuilder.RenameTable(
                name: "UserGroupInvitations",
                newName: "UserGroupInvitation");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameTable(
                name: "GroupMembers",
                newName: "GroupMember");

            migrationBuilder.RenameTable(
                name: "GroupMemberDestinationVotes",
                newName: "GroupMemberDestinationVote");

            migrationBuilder.RenameTable(
                name: "GroupMatches",
                newName: "GroupMatch");

            migrationBuilder.RenameTable(
                name: "GroupInvitations",
                newName: "GroupInvitation");

            migrationBuilder.RenameIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreference",
                newName: "IX_UserPreference_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupInvitations_UserId",
                table: "UserGroupInvitation",
                newName: "IX_UserGroupInvitation_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupInvitations_GroupInvitationId",
                table: "UserGroupInvitation",
                newName: "IX_UserGroupInvitation_GroupInvitationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMembers_UserId",
                table: "GroupMember",
                newName: "IX_GroupMember_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMember",
                newName: "IX_GroupMember_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberDestinationVotes_GroupMemberId",
                table: "GroupMemberDestinationVote",
                newName: "IX_GroupMemberDestinationVote_GroupMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberDestinationVotes_DestinationId",
                table: "GroupMemberDestinationVote",
                newName: "IX_GroupMemberDestinationVote_DestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMatches_GroupId",
                table: "GroupMatch",
                newName: "IX_GroupMatch_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMatches_DestinationId",
                table: "GroupMatch",
                newName: "IX_GroupMatch_DestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupInvitations_GroupId",
                table: "GroupInvitation",
                newName: "IX_GroupInvitation_GroupId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreference",
                table: "UserPreference",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroupInvitation",
                table: "UserGroupInvitation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMember",
                table: "GroupMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMemberDestinationVote",
                table: "GroupMemberDestinationVote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMatch",
                table: "GroupMatch",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupInvitation",
                table: "GroupInvitation",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupInvitation_Group_GroupId",
                table: "GroupInvitation",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMatch_Destinations_DestinationId",
                table: "GroupMatch",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMatch_Group_GroupId",
                table: "GroupMatch",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMember_Group_GroupId",
                table: "GroupMember",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMember_Users_UserId",
                table: "GroupMember",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberDestinationVote_Destinations_DestinationId",
                table: "GroupMemberDestinationVote",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberDestinationVote_GroupMember_GroupMemberId",
                table: "GroupMemberDestinationVote",
                column: "GroupMemberId",
                principalTable: "GroupMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupInvitation_GroupInvitation_GroupInvitationId",
                table: "UserGroupInvitation",
                column: "GroupInvitationId",
                principalTable: "GroupInvitation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupInvitation_Users_UserId",
                table: "UserGroupInvitation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreference_Users_UserId",
                table: "UserPreference",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupInvitation_Group_GroupId",
                table: "GroupInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMatch_Destinations_DestinationId",
                table: "GroupMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMatch_Group_GroupId",
                table: "GroupMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMember_Group_GroupId",
                table: "GroupMember");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMember_Users_UserId",
                table: "GroupMember");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberDestinationVote_Destinations_DestinationId",
                table: "GroupMemberDestinationVote");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberDestinationVote_GroupMember_GroupMemberId",
                table: "GroupMemberDestinationVote");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupInvitation_GroupInvitation_GroupInvitationId",
                table: "UserGroupInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupInvitation_Users_UserId",
                table: "UserGroupInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreference_Users_UserId",
                table: "UserPreference");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreference",
                table: "UserPreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroupInvitation",
                table: "UserGroupInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMemberDestinationVote",
                table: "GroupMemberDestinationVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMember",
                table: "GroupMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMatch",
                table: "GroupMatch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupInvitation",
                table: "GroupInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UserPreference",
                newName: "UserPreferences");

            migrationBuilder.RenameTable(
                name: "UserGroupInvitation",
                newName: "UserGroupInvitations");

            migrationBuilder.RenameTable(
                name: "GroupMemberDestinationVote",
                newName: "GroupMemberDestinationVotes");

            migrationBuilder.RenameTable(
                name: "GroupMember",
                newName: "GroupMembers");

            migrationBuilder.RenameTable(
                name: "GroupMatch",
                newName: "GroupMatches");

            migrationBuilder.RenameTable(
                name: "GroupInvitation",
                newName: "GroupInvitations");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameIndex(
                name: "IX_UserPreference_UserId",
                table: "UserPreferences",
                newName: "IX_UserPreferences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupInvitation_UserId",
                table: "UserGroupInvitations",
                newName: "IX_UserGroupInvitations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupInvitation_GroupInvitationId",
                table: "UserGroupInvitations",
                newName: "IX_UserGroupInvitations_GroupInvitationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberDestinationVote_GroupMemberId",
                table: "GroupMemberDestinationVotes",
                newName: "IX_GroupMemberDestinationVotes_GroupMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberDestinationVote_DestinationId",
                table: "GroupMemberDestinationVotes",
                newName: "IX_GroupMemberDestinationVotes_DestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMember_UserId",
                table: "GroupMembers",
                newName: "IX_GroupMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMember_GroupId",
                table: "GroupMembers",
                newName: "IX_GroupMembers_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMatch_GroupId",
                table: "GroupMatches",
                newName: "IX_GroupMatches_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMatch_DestinationId",
                table: "GroupMatches",
                newName: "IX_GroupMatches_DestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupInvitation_GroupId",
                table: "GroupInvitations",
                newName: "IX_GroupInvitations_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferences",
                table: "UserPreferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroupInvitations",
                table: "UserGroupInvitations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMemberDestinationVotes",
                table: "GroupMemberDestinationVotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMatches",
                table: "GroupMatches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupInvitations",
                table: "GroupInvitations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupInvitations_Groups_GroupId",
                table: "GroupInvitations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMatches_Destinations_DestinationId",
                table: "GroupMatches",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMatches_Groups_GroupId",
                table: "GroupMatches",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberDestinationVotes_Destinations_DestinationId",
                table: "GroupMemberDestinationVotes",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberDestinationVotes_GroupMembers_GroupMemberId",
                table: "GroupMemberDestinationVotes",
                column: "GroupMemberId",
                principalTable: "GroupMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Groups_GroupId",
                table: "GroupMembers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Users_UserId",
                table: "GroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupInvitations_GroupInvitations_GroupInvitationId",
                table: "UserGroupInvitations",
                column: "GroupInvitationId",
                principalTable: "GroupInvitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupInvitations_Users_UserId",
                table: "UserGroupInvitations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
