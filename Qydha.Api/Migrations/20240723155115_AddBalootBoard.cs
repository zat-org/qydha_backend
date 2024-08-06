using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBalootBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "baloot_boards",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_baloot_boards", x => new { x.user_id, x.id });
                    table.ForeignKey(
                        name: "FK_baloot_boards_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Insert a board for each user using a raw SQL query
            migrationBuilder.Sql(@"
                INSERT INTO baloot_boards (id, user_id)
                SELECT uuid_generate_v4(), id
                FROM users;
            ");
            migrationBuilder.CreateIndex(
                name: "IX_baloot_boards_user_id",
                table: "baloot_boards",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "baloot_boards");
        }
    }
}
