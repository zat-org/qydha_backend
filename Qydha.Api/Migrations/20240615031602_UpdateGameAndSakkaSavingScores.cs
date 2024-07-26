using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameAndSakkaSavingScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "them_score",
                table: "baloot_sakkas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "us_score",
                table: "baloot_sakkas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "them_game_score",
                table: "baloot_games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "us_game_score",
                table: "baloot_games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "them_score",
                table: "baloot_sakkas");

            migrationBuilder.DropColumn(
                name: "us_score",
                table: "baloot_sakkas");

            migrationBuilder.DropColumn(
                name: "them_game_score",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "us_game_score",
                table: "baloot_games");
        }
    }
}
