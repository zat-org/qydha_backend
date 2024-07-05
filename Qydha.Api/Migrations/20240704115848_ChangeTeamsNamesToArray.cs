using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTeamsNamesToArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Roles",
                table: "users",
                newName: "roles");

            // Step 1: Add a temporary column to store the text[] data
            migrationBuilder.AddColumn<string[]>(
                name: "players_names_temp",
                table: "user_general_settings",
                type: "text[]",
                nullable: false,
                defaultValue: new string[] { });

            // Step 2: Copy data from jsonb to text[]
            migrationBuilder.Sql(@"
                UPDATE user_general_settings AS ugs
                SET players_names_temp = subquery.text_array
                FROM (
                    SELECT user_id, array_agg(element::text) AS text_array
                    FROM user_general_settings,
                    LATERAL jsonb_array_elements_text(user_general_settings.players_names) AS element
                    GROUP BY user_id
                ) AS subquery
                WHERE ugs.user_id = subquery.user_id;
            ");

            // Step 3: Alter the original column to text[]
            migrationBuilder.DropColumn(
               name: "players_names",
               table: "user_general_settings");
            migrationBuilder.AddColumn<string[]>(
                name: "players_names",
                table: "user_general_settings",
                type: "text[]",
                nullable: false,
                defaultValue: new string[] { }); // Adjust defaultValue as needed

            // Step 4: Copy data from temp column back to original column
            migrationBuilder.Sql(@"
                UPDATE user_general_settings
                SET players_names = players_names_temp
            ");

            // Step 5: Drop the temporary column
            migrationBuilder.DropColumn(
                name: "players_names_temp",
                table: "user_general_settings");

            // =================

            // Step 1: Add a temporary column to store the text[] data
            migrationBuilder.AddColumn<string[]>(
                name: "teams_names_temp",
                table: "user_general_settings",
                type: "text[]",
                nullable: false,
                defaultValue: new string[] { });

            // Step 2: Copy data from jsonb to text[]
            migrationBuilder.Sql(@"
                UPDATE user_general_settings AS ugs
                SET teams_names_temp = subquery.text_array
                FROM (
                    SELECT user_id, array_agg(element::text) AS text_array
                    FROM user_general_settings,
                    LATERAL jsonb_array_elements_text(user_general_settings.teams_names) AS element
                    GROUP BY user_id
                ) AS subquery
                WHERE ugs.user_id = subquery.user_id;
            ");

            // Step 3: Alter the original column to text[]
            migrationBuilder.DropColumn(
                name: "teams_names",
                table: "user_general_settings");

            migrationBuilder.AddColumn<string[]>(
                name: "teams_names",
                table: "user_general_settings",
                type: "text[]",
                nullable: false,
                defaultValue: new string[] { }); // Adjust defaultValue as needed

            // Step 4: Copy data from temp column back to original column
            migrationBuilder.Sql(@"
                UPDATE user_general_settings
                SET teams_names = teams_names_temp
            ");

            // Step 5: Drop the temporary column
            migrationBuilder.DropColumn(
                name: "teams_names_temp",
                table: "user_general_settings");

            // =============================

            migrationBuilder.UpdateData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "players_names", "teams_names" },
                values: new object[] { new List<string>(), new List<string>() });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "roles",
                table: "users",
                newName: "Roles");

            //===========================
            migrationBuilder.AddColumn<string>(
                name: "teams_names_temp",
                table: "user_general_settings",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.Sql(@"
                UPDATE user_general_settings 
                SET teams_names_temp = to_jsonb(teams_names);
            ");

            migrationBuilder.DropColumn(
                name: "teams_names",
                table: "user_general_settings");
            migrationBuilder.AddColumn<string>(
                name: "teams_names",
                table: "user_general_settings",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");
            migrationBuilder.Sql(@"
                UPDATE user_general_settings
                SET teams_names = teams_names_temp
            ");
            migrationBuilder.DropColumn(
                name: "teams_names_temp",
                table: "user_general_settings");
            // ====================================
            migrationBuilder.AddColumn<string>(
               name: "players_names_temp",
               table: "user_general_settings",
               type: "jsonb",
               nullable: false,
               defaultValueSql: "'[]'::jsonb");

            migrationBuilder.Sql(@"
                UPDATE user_general_settings 
                SET players_names_temp = to_jsonb(players_names);
            ");
            migrationBuilder.DropColumn(
                name: "players_names",
                table: "user_general_settings");

            migrationBuilder.AddColumn<string>(
                name: "players_names",
                table: "user_general_settings",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.Sql(@"
                UPDATE user_general_settings
                SET players_names = players_names_temp
            ");

            migrationBuilder.DropColumn(
                name: "players_names_temp",
                table: "user_general_settings");

            migrationBuilder.UpdateData(
                table: "user_general_settings",
                keyColumn: "user_id",
                keyValue: new Guid("d2705466-4304-4830-b48a-3e44e031927e"),
                columns: new[] { "players_names", "teams_names" },
                values: new object[] { "[]", "[]" });
        }
    }
}
