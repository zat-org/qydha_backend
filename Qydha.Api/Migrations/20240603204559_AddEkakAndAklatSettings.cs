using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEkakAndAklatSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_ekak_aklat_shown",
                table: "user_baloot_settings",
                newName: "is_ekak_shown");

            migrationBuilder.AddColumn<bool>(
                name: "is_aklat_shown",
                table: "user_baloot_settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_aklat_shown",
                table: "user_baloot_settings");

            migrationBuilder.RenameColumn(
                name: "is_ekak_shown",
                table: "user_baloot_settings",
                newName: "is_ekak_aklat_shown");
        }
    }
}
