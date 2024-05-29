using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSakkaCountForBalootSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sakkas_count",
                table: "user_baloot_settings",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sakkas_count",
                table: "user_baloot_settings");
        }
    }
}
