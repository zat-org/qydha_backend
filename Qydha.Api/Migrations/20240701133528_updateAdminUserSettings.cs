using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class updateAdminUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "user_baloot_settings",
                column: "user_id",
                value: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));

            migrationBuilder.InsertData(
                table: "user_general_settings",
                column: "user_id",
                value: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));

            migrationBuilder.InsertData(
                table: "user_hand_settings",
                column: "user_id",
                value: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_baloot_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));

            migrationBuilder.DeleteData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));

            migrationBuilder.DeleteData(
                table: "user_hand_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));
        }
    }
}
