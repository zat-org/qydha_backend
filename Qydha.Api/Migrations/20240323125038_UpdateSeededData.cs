using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "visibility",
                table: "notifications_data",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "$2a$11$V0A5.EYwXlFUjK3RIis3...A9rfzUm.mO.88MUYW9.uHSZLjURNsC" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "visibility",
                table: "notifications_data",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "created_at", "password_hash" },
                values: new object[] { new DateTime(2024, 3, 22, 15, 55, 39, 229, DateTimeKind.Utc).AddTicks(5723), "$2a$11$YSqSTrYLDie0ebfuHwTq.OnODD3S5XH6Zh5YP7wX.Li3/RYkqzYB." });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4751), 3 });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4762), 3 });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4766), 3 });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4770), 3 });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4778), 3 });
        }
    }
}
