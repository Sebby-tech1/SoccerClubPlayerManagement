using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerClubPlayerManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddBallControlTrait : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Traits",
                columns: new[] { "TraitId", "Description", "Name", "PositionId" },
                values: new object[] { 16, null, "Ball Control", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 16);
        }
    }
}
