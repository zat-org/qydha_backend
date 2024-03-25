using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class TemplateValuesForNotificationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sending_mechanism",
                table: "notifications_data",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "template_values",
                table: "notifications_data",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                column: "created_at",
                value: new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "description", "sending_mechanism", "template_values", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "نتمنى لك تجربة جميلة، ارسلنا لك هدية بقسم المتجر 😉", "Automatic", "{}", "Private" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "sending_mechanism", "template_values", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Automatic", "{}", "Private" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "description", "sending_mechanism", "template_values", "title", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "شيك على المتجر .. تتهنى ♥", "Automatic", "{}", "وصلتك هدية.. 🎁", "Private" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "description", "sending_mechanism", "template_values", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "نتمنى لك تجربة ممتعة ♥", "Automatic", "{}", "Private" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "sending_mechanism", "template_values", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Automatic", "{}", "Private" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "sending_mechanism", "template_values", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Automatic", "{}", "Private" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sending_mechanism",
                table: "notifications_data");

            migrationBuilder.DropColumn(
                name: "template_values",
                table: "notifications_data");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                column: "created_at",
                value: new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "description", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "نتمنى لك تجربة جميلة، ارسلنا لك هدية بقسم المتجر😉", "Automatic" });

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
                columns: new[] { "created_at", "description", "title", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "شيك على المتجر .. تتهنى♥", "وصلتك هدية..🎁 ", "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "description", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "نتمنى لك تجربة ممتعة♥", "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });

            migrationBuilder.UpdateData(
                table: "notifications_data",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "created_at", "visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Automatic" });
        }
    }
}
