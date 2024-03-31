using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBirthDateToDateAndRemoveRelationBetweenUserAndRegistrationOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registration_otp_request_users_UserId",
                table: "registration_otp_request");

            migrationBuilder.DropIndex(
                name: "IX_registration_otp_request_UserId",
                table: "registration_otp_request");

            migrationBuilder.DropColumn(
                name: "free_subscription_used",
                table: "users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "registration_otp_request");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "birth_date",
                table: "users",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "birth_date",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "free_subscription_used",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "registration_otp_request",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_registration_otp_request_UserId",
                table: "registration_otp_request",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_otp_request_users_UserId",
                table: "registration_otp_request",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
