using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToUserAndRemoveUnNecessaryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "user_baloot_settings_user_id_fkey",
                table: "user_baloot_settings");

            migrationBuilder.DropForeignKey(
                name: "user_general_settings_user_id_fkey",
                table: "user_general_settings");

            migrationBuilder.DropForeignKey(
                name: "user_hand_settings_user_id_fkey",
                table: "user_hand_settings");

            migrationBuilder.DropPrimaryKey(
                name: "user_hand_settings_pkey",
                table: "user_hand_settings");

            migrationBuilder.DropPrimaryKey(
                name: "user_general_settings_pkey",
                table: "user_general_settings");

            migrationBuilder.DropPrimaryKey(
                name: "user_baloot_settings_pkey",
                table: "user_baloot_settings");

            migrationBuilder.DropColumn(
                name: "is_anonymous",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_email_confirmed",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_phone_confirmed",
                table: "users");

            migrationBuilder.AddColumn<int[]>(
                name: "Roles",
                table: "users",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AlterColumn<string>(
                name: "normalized_username",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                computedColumnSql: "UPPER(username)",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "normalized_email",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                computedColumnSql: "UPPER(email)",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_hand_settings",
                table: "user_hand_settings",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_general_settings",
                table: "user_general_settings",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_baloot_settings",
                table: "user_baloot_settings",
                column: "user_id");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar_path", "avatar_url", "birth_date", "created_on", "email", "expire_date", "fcm_token", "last_login", "name", "password_hash", "phone", "Roles", "username" },
                values: new object[] { new Guid("d2705466-4304-4830-b48a-3e44e031927e"), null, null, null, new DateTimeOffset(new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, null, "$2a$11$V0A5.EYwXlFUjK3RIis3...A9rfzUm.mO.88MUYW9.uHSZLjURNsC", "+201555330346", new[] { 1 }, "Admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_baloot_settings_users_user_id",
                table: "user_baloot_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_general_settings_users_user_id",
                table: "user_general_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_hand_settings_users_user_id",
                table: "user_hand_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_baloot_settings_users_user_id",
                table: "user_baloot_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_user_general_settings_users_user_id",
                table: "user_general_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_user_hand_settings_users_user_id",
                table: "user_hand_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_hand_settings",
                table: "user_hand_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_general_settings",
                table: "user_general_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_baloot_settings",
                table: "user_baloot_settings");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"));

            migrationBuilder.DropColumn(
                name: "Roles",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "normalized_username",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComputedColumnSql: "UPPER(username)");

            migrationBuilder.AlterColumn<string>(
                name: "normalized_email",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComputedColumnSql: "UPPER(email)");

            migrationBuilder.AddColumn<bool>(
                name: "is_anonymous",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_email_confirmed",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_phone_confirmed",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "user_hand_settings_pkey",
                table: "user_hand_settings",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "user_general_settings_pkey",
                table: "user_general_settings",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "user_baloot_settings_pkey",
                table: "user_baloot_settings",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "user_baloot_settings_user_id_fkey",
                table: "user_baloot_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "user_general_settings_user_id_fkey",
                table: "user_general_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "user_hand_settings_user_id_fkey",
                table: "user_hand_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
