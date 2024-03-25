using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "id", "created_at", "normalized_username", "password_hash", "role", "username" },
                values: new object[] { new Guid("aaf6ac5a-cbea-43d3-b5f8-ed200d55fb6f"), new DateTime(2024, 3, 22, 3, 9, 5, 496, DateTimeKind.Utc).AddTicks(8463), "ADMIN", "$2a$11$hHvvaG5hUHkfu7IKADkwK.5jTKeUKzeORqxnRIazqFVvbYjoEa7MG", "SuperAdmin", "Admin" });

            migrationBuilder.InsertData(
                table: "notifications_data",
                columns: new[] { "id", "action_path", "action_type", "created_at", "description", "payload", "title", "visibility" },
                values: new object[,]
                {
                    { 1, "_", 1, new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(772), "نتمنى لك تجربة جميلة، ارسلنا لك هدية بقسم المتجر😉", "{\"Image\":null}", "مرحباً بك في قيدها ♥", 3 },
                    { 2, "_", 1, new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(786), "نتمنى لك تجربة جميلة، لا تنسى قيدها ليس مجرد حاسبة", "{\"Image\":null}", "شكرا لثقتك بقيدها..", 3 },
                    { 3, "_", 1, new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(789), "شيك على المتجر .. تتهنى♥", "{\"Image\":null}", "وصلتك هدية..🎁 ", 3 },
                    { 4, "_", 1, new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(792), "نتمنى لك تجربة ممتعة♥", "{\"Image\":null}", "تستاهل ما جاك", 3 },
                    { 5, "_", 1, new DateTime(2024, 3, 22, 3, 9, 5, 790, DateTimeKind.Utc).AddTicks(794), "إذا عجبك التطبيق لا تنسى تنشره بين أخوياك", "{\"Image\":null}", "تم تفعيل الكود", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("aaf6ac5a-cbea-43d3-b5f8-ed200d55fb6f"));

            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5);
        }
    }
}
