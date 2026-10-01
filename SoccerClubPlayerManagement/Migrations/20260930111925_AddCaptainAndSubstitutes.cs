using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerClubPlayerManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCaptainAndSubstitutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaptainPlayerId",
                table: "Lineups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoachName",
                table: "Lineups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LineupSubstitutes",
                columns: table => new
                {
                    LineupSubstituteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineupId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: true),
                    SubOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineupSubstitutes", x => x.LineupSubstituteId);
                    table.ForeignKey(
                        name: "FK_LineupSubstitutes_Lineups_LineupId",
                        column: x => x.LineupId,
                        principalTable: "Lineups",
                        principalColumn: "LineupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineupSubstitutes_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lineups_CaptainPlayerId",
                table: "Lineups",
                column: "CaptainPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupSubstitutes_LineupId",
                table: "LineupSubstitutes",
                column: "LineupId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupSubstitutes_PlayerId",
                table: "LineupSubstitutes",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lineups_Players_CaptainPlayerId",
                table: "Lineups",
                column: "CaptainPlayerId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lineups_Players_CaptainPlayerId",
                table: "Lineups");

            migrationBuilder.DropTable(
                name: "LineupSubstitutes");

            migrationBuilder.DropIndex(
                name: "IX_Lineups_CaptainPlayerId",
                table: "Lineups");

            migrationBuilder.DropColumn(
                name: "CaptainPlayerId",
                table: "Lineups");

            migrationBuilder.DropColumn(
                name: "CoachName",
                table: "Lineups");
        }
    }
}
