using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFcmNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("aaf6ac5a-cbea-43d3-b5f8-ed200d55fb6f"));

            migrationBuilder.AlterColumn<string>(
                name: "fcm_token",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "registration_otp_request",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "id", "created_at", "normalized_username", "password_hash", "role", "username" },
                values: new object[] { new Guid("d2705466-4304-4830-b48a-3e44e031927e"), new DateTime(2024, 3, 22, 15, 55, 39, 229, DateTimeKind.Utc).AddTicks(5723), "ADMIN", "$2a$11$YSqSTrYLDie0ebfuHwTq.OnODD3S5XH6Zh5YP7wX.Li3/RYkqzYB.", "SuperAdmin", "Admin" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4751));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4762));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4766));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4770));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 15, 55, 39, 487, DateTimeKind.Utc).AddTicks(4778));

            migrationBuilder.CreateIndex(
                name: "IX_registration_otp_request_UserId",
                table: "registration_otp_request",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_otp_request_users_UserId",
                table: "registration_otp_request",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registration_otp_request_users_UserId",
                table: "registration_otp_request");

            migrationBuilder.DropIndex(
                name: "IX_registration_otp_request_UserId",
                table: "registration_otp_request");

            migrationBuilder.DeleteData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "registration_otp_request");

            migrationBuilder.AlterColumn<string>(
                name: "fcm_token",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "id", "created_at", "normalized_username", "password_hash", "role", "username" },
                values: new object[] { new Guid("aaf6ac5a-cbea-43d3-b5f8-ed200d55fb6f"), new DateTime(2024, 3, 22, 3, 9, 5, 496, DateTimeKind.Utc).AddTicks(8463), "ADMIN", "$2a$11$hHvvaG5hUHkfu7IKADkwK.5jTKeUKzeORqxnRIazqFVvbYjoEa7MG", "SuperAdmin", "Admin" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(772));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(786));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(789));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(792));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5,
                column: "created_at",
                value: new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(794));
        }
    }
}
