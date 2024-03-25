using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAppAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "app_assets",
                columns: new[] { "asset_key", "asset_data" },
                values: new object[,]
                {
                    { "baloot_book", "{}" },
                    { "popup", "{}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "app_assets",
                keyColumn: "asset_key",
                keyValue: "baloot_book");

            migrationBuilder.DeleteData(
                table: "app_assets",
                keyColumn: "asset_key",
                keyValue: "popup");
        }
    }
}
