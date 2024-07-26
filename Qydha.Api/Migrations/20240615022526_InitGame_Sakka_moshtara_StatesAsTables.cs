using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitGame_Sakka_moshtara_StatesAsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "game_state",
                table: "baloot_games");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ended_at",
                table: "baloot_games",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "max_sakka_per_game",
                table: "baloot_games",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "pausing_intervals",
                table: "baloot_games",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "started_at",
                table: "baloot_games",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "baloot_games",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "them_name",
                table: "baloot_games",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "us_name",
                table: "baloot_games",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "winner",
                table: "baloot_games",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "baloot_sakkas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    baloot_game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    is_mashdoda = table.Column<bool>(type: "boolean", nullable: false),
                    winner = table.Column<string>(type: "text", nullable: true),
                    draw_handler = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    pausing_intervals = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_baloot_sakkas", x => x.id);
                    table.ForeignKey(
                        name: "FK_baloot_sakkas_baloot_games_baloot_game_id",
                        column: x => x.baloot_game_id,
                        principalTable: "baloot_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "baloot_moshtaras",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    baloot_sakka_id = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    pausing_intervals = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    state = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_baloot_moshtaras", x => x.id);
                    table.ForeignKey(
                        name: "FK_baloot_moshtaras_baloot_sakkas_baloot_sakka_id",
                        column: x => x.baloot_sakka_id,
                        principalTable: "baloot_sakkas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_baloot_moshtaras_baloot_sakka_id",
                table: "baloot_moshtaras",
                column: "baloot_sakka_id");

            migrationBuilder.CreateIndex(
                name: "IX_baloot_sakkas_baloot_game_id",
                table: "baloot_sakkas",
                column: "baloot_game_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "baloot_moshtaras");

            migrationBuilder.DropTable(
                name: "baloot_sakkas");

            migrationBuilder.DropColumn(
                name: "ended_at",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "max_sakka_per_game",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "pausing_intervals",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "started_at",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "state",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "them_name",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "us_name",
                table: "baloot_games");

            migrationBuilder.DropColumn(
                name: "winner",
                table: "baloot_games");

            migrationBuilder.AddColumn<string>(
                name: "game_state",
                table: "baloot_games",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");
        }
    }
}
