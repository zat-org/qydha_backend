using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOnDeleteForGameOwnerAndModerator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderator_baloot_games_link",
                table: "baloot_games");

            migrationBuilder.DropForeignKey(
                name: "fk_owner_baloot_games_link",
                table: "baloot_games");

            migrationBuilder.UpdateData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "players_names", "teams_names" },
                values: new object[] { new List<string>(), new List<string>() });

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_baloot_games_link",
                table: "baloot_games",
                column: "moderator_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_owner_baloot_games_link",
                table: "baloot_games",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_moderator_baloot_games_link",
                table: "baloot_games");

            migrationBuilder.DropForeignKey(
                name: "fk_owner_baloot_games_link",
                table: "baloot_games");

            migrationBuilder.UpdateData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "players_names", "teams_names" },
                values: new object[] { new List<string>(), new List<string>() });

            migrationBuilder.AddForeignKey(
                name: "fk_moderator_baloot_games_link",
                table: "baloot_games",
                column: "moderator_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_owner_baloot_games_link",
                table: "baloot_games",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
