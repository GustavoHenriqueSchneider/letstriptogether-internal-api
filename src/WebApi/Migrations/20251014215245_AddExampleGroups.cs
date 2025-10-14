using Microsoft.EntityFrameworkCore.Migrations;
using WebApi.Security;
using WebApi.Services.Implementations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddExampleGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name", "TripExpectedDate", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Test Group", DateTime.UtcNow.AddYears(3), DateTime.UtcNow, null },
                    { Guid.NewGuid(), "Test Group 2", DateTime.UtcNow.AddMonths(5), DateTime.UtcNow, DateTime.UtcNow.AddSeconds(5) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
