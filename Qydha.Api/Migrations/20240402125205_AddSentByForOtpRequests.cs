using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSentByForOtpRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user",
                table: "purchases");

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                table: "update_phone_requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                table: "update_email_requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                table: "registration_otp_request",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                table: "phone_authentication_requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_user",
                table: "purchases",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "SentBy",
                table: "update_phone_requests");

            migrationBuilder.DropColumn(
                name: "SentBy",
                table: "update_email_requests");

            migrationBuilder.DropColumn(
                name: "SentBy",
                table: "registration_otp_request");

            migrationBuilder.DropColumn(
                name: "SentBy",
                table: "phone_authentication_requests");

            migrationBuilder.AddForeignKey(
                name: "fk_user",
                table: "purchases",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
