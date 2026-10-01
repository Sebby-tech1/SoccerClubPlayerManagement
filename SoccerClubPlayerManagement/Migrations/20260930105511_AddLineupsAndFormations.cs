using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoccerClubPlayerManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddLineupsAndFormations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formations",
                columns: table => new
                {
                    FormationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formations", x => x.FormationId);
                });

            migrationBuilder.CreateTable(
                name: "FormationSlots",
                columns: table => new
                {
                    FormationSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SlotOrder = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormationSlots", x => x.FormationSlotId);
                    table.ForeignKey(
                        name: "FK_FormationSlots_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "FormationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lineups",
                columns: table => new
                {
                    LineupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineups", x => x.LineupId);
                    table.ForeignKey(
                        name: "FK_Lineups_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "FormationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineupSlots",
                columns: table => new
                {
                    LineupSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineupId = table.Column<int>(type: "int", nullable: false),
                    FormationSlotId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineupSlots", x => x.LineupSlotId);
                    table.ForeignKey(
                        name: "FK_LineupSlots_FormationSlots_FormationSlotId",
                        column: x => x.FormationSlotId,
                        principalTable: "FormationSlots",
                        principalColumn: "FormationSlotId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineupSlots_Lineups_LineupId",
                        column: x => x.LineupId,
                        principalTable: "Lineups",
                        principalColumn: "LineupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineupSlots_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Formations",
                columns: new[] { "FormationId", "Name" },
                values: new object[,]
                {
                    { 1, "4-4-2" },
                    { 2, "4-3-3" },
                    { 3, "3-5-2" },
                    { 4, "4-2-3-1" }
                });

            migrationBuilder.InsertData(
                table: "FormationSlots",
                columns: new[] { "FormationSlotId", "FormationId", "Label", "SlotOrder", "X", "Y" },
                values: new object[,]
                {
                    { 1, 1, "GK", 1, 50, 95 },
                    { 2, 1, "LB", 2, 15, 75 },
                    { 3, 1, "CB", 3, 35, 80 },
                    { 4, 1, "CB", 4, 65, 80 },
                    { 5, 1, "RB", 5, 85, 75 },
                    { 6, 1, "LM", 6, 15, 45 },
                    { 7, 1, "CM", 7, 35, 50 },
                    { 8, 1, "CM", 8, 65, 50 },
                    { 9, 1, "RM", 9, 85, 45 },
                    { 10, 1, "ST", 10, 35, 15 },
                    { 11, 1, "ST", 11, 65, 15 },
                    { 12, 2, "GK", 1, 50, 95 },
                    { 13, 2, "LB", 2, 15, 75 },
                    { 14, 2, "CB", 3, 35, 80 },
                    { 15, 2, "CB", 4, 65, 80 },
                    { 16, 2, "RB", 5, 85, 75 },
                    { 17, 2, "CM", 6, 30, 50 },
                    { 18, 2, "CM", 7, 50, 55 },
                    { 19, 2, "CM", 8, 70, 50 },
                    { 20, 2, "LW", 9, 15, 20 },
                    { 21, 2, "ST", 10, 50, 10 },
                    { 22, 2, "RW", 11, 85, 20 },
                    { 23, 3, "GK", 1, 50, 95 },
                    { 24, 3, "CB", 2, 30, 80 },
                    { 25, 3, "CB", 3, 50, 85 },
                    { 26, 3, "CB", 4, 70, 80 },
                    { 27, 3, "LM", 5, 10, 50 },
                    { 28, 3, "CM", 6, 35, 55 },
                    { 29, 3, "CM", 7, 50, 60 },
                    { 30, 3, "CM", 8, 65, 55 },
                    { 31, 3, "RM", 9, 90, 50 },
                    { 32, 3, "ST", 10, 35, 15 },
                    { 33, 3, "ST", 11, 65, 15 },
                    { 34, 4, "GK", 1, 50, 95 },
                    { 35, 4, "LB", 2, 15, 75 },
                    { 36, 4, "CB", 3, 35, 80 },
                    { 37, 4, "CB", 4, 65, 80 },
                    { 38, 4, "RB", 5, 85, 75 },
                    { 39, 4, "CDM", 6, 35, 60 },
                    { 40, 4, "CDM", 7, 65, 60 },
                    { 41, 4, "LW", 8, 15, 35 },
                    { 42, 4, "CAM", 9, 50, 30 },
                    { 43, 4, "RW", 10, 85, 35 },
                    { 44, 4, "ST", 11, 50, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormationSlots_FormationId",
                table: "FormationSlots",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineups_FormationId",
                table: "Lineups",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupSlots_FormationSlotId",
                table: "LineupSlots",
                column: "FormationSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupSlots_LineupId",
                table: "LineupSlots",
                column: "LineupId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupSlots_PlayerId",
                table: "LineupSlots",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineupSlots");

            migrationBuilder.DropTable(
                name: "FormationSlots");

            migrationBuilder.DropTable(
                name: "Lineups");

            migrationBuilder.DropTable(
                name: "Formations");
        }
    }
}
