using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedLoginWithQydhaNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "template_values",
                table: "notifications_users_link",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.InsertData(
                table: "notifications_data",
                columns: new[] { "id", "action_path", "action_type", "created_at", "description", "payload", "title", "visibility" },
                values: new object[] { 6, "_", 1, new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "رمز الدخول هو {Otp} تستطيع استخدامه لتسجيل الدخول على {ServiceName} باستخدام حسابك بتطبيق قيدها", "{\"Image\":null}", "تسجيل دخول الى {ServiceName}", "Automatic" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "template_values",
                table: "notifications_users_link");
        }
    }
}
