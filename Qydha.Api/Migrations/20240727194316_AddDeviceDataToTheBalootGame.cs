using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceDataToTheBalootGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "device_data",
                table: "baloot_games",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{\"Environment\":null,\"AppVersion\":null,\"Platform\":null,\"DeviceName\":null,\"DeviceId\":null}'::jsonb");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device_data",
                table: "baloot_games");
        }
    }
}
