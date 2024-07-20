using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQydhaServiceAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "service_accounts",
                columns: new[] { "id", "description", "name", "permissions" },
                values: new object[] { new Guid("62dd9f79-a8a1-4031-ba55-c2ddca88b0bb"), "qydha primary service account", "قيدها", new[] { 0, 6, 1, 4, 5 } });

            migrationBuilder.UpdateData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "players_names", "teams_names" },
                values: new object[] { new List<string>(), new List<string>() });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "service_accounts",
                keyColumn: "id",
                keyValue: new Guid("62dd9f79-a8a1-4031-ba55-c2ddca88b0bb"));

            migrationBuilder.UpdateData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "players_names", "teams_names" },
                values: new object[] { new List<string>(), new List<string>() });
        }
    }
}
