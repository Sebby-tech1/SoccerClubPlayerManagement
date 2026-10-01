using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoccerClubPlayerManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerAttributesAndPositionTraits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "Traits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeightFeet",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeightInches",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredFoot",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 1,
                column: "PositionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 2,
                column: "PositionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 3,
                column: "PositionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 4,
                column: "PositionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 5,
                column: "PositionId",
                value: null);

            migrationBuilder.InsertData(
                table: "Traits",
                columns: new[] { "TraitId", "Description", "Name", "PositionId" },
                values: new object[,]
                {
                    { 6, null, "Tackling", null },
                    { 7, null, "Marking", null },
                    { 8, null, "Heading", null },
                    { 9, null, "Agility", null },
                    { 10, null, "Vision", null },
                    { 11, null, "Skill Moves", null },
                    { 12, null, "Diving", 1 },
                    { 13, null, "Reflexes", 1 },
                    { 14, null, "Handling", 1 },
                    { 15, null, "Distribution", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Traits_PositionId",
                table: "Traits",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Traits_Positions_PositionId",
                table: "Traits",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Traits_Positions_PositionId",
                table: "Traits");

            migrationBuilder.DropIndex(
                name: "IX_Traits_PositionId",
                table: "Traits");

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Traits",
                keyColumn: "TraitId",
                keyValue: 15);

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "HeightFeet",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HeightInches",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PreferredFoot",
                table: "Players");
        }
    }
}
