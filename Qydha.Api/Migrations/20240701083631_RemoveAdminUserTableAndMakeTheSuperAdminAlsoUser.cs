using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminUserTableAndMakeTheSuperAdminAlsoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                column: "Roles",
                value: new[] { 1, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    normalized_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("admins_pkey", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "id", "created_at", "normalized_username", "password_hash", "role", "username" },
                values: new object[] { new Guid("d2705466-4304-4830-b48a-3e44e031927e"), new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ADMIN", "$2a$11$V0A5.EYwXlFUjK3RIis3...A9rfzUm.mO.88MUYW9.uHSZLjURNsC", "SuperAdmin", "Admin" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                column: "Roles",
                value: new[] { 1 });

            migrationBuilder.CreateIndex(
                name: "admins_normalized_username_key",
                table: "admins",
                column: "normalized_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "admins_username_key",
                table: "admins",
                column: "username",
                unique: true);
        }
    }
}
