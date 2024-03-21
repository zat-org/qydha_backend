using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Qydha.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitBeforeEf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    normalized_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("admins_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_assets",
                columns: table => new
                {
                    asset_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    asset_data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("app_assets_pkey", x => x.asset_key);
                });

            migrationBuilder.CreateTable(
                name: "influencer_codes_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    max_codes_per_user_in_group = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("influencer_codes_categories_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    action_path = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    anonymous_clicks = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    payload = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_data_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "registration_otp_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying", nullable: false),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fcm_token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("registration_otp_request_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    password_hash = table.Column<string>(type: "character varying", nullable: false),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_anonymous = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_phone_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    avatar_url = table.Column<string>(type: "character varying", nullable: true),
                    avatar_path = table.Column<string>(type: "character varying", nullable: true),
                    expire_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    free_subscription_used = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    fcm_token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    normalized_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "influencer_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expire_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    number_of_days = table.Column<int>(type: "integer", nullable: false),
                    max_influenced_users_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    category_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("influencer_codes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_influencer_code_categories",
                        column: x => x.category_id,
                        principalTable: "influencer_codes_categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "login_with_qydha_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("login_with_qydha_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications_users_link",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notification_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_users_link_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_at_notification_link_table",
                        column: x => x.notification_id,
                        principalTable: "notifications_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_at_notification_link_table",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phone_authentication_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phone_authentication_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_phone_authentication_requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    iaphub_purchase_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    purchase_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    productsku = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    number_of_days = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("purchases_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "update_email_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("update_email_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_update_email_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "update_phone_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("update_phone_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_update_phone_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_baloot_settings",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_flipped = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_advanced_recording = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_sakkah_mashdodah_mode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    show_who_won_dialog_on_draw = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_numbers_sound_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_comments_sound_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_baloot_settings_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "user_baloot_settings_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_general_settings",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enable_vibration = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    players_names = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    teams_names = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_general_settings_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "user_general_settings_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_hand_settings",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rounds_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 7),
                    max_limit = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    teams_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    players_count_in_team = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    win_using_zat = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    takweesh_points = table.Column<int>(type: "integer", nullable: false, defaultValue: 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_hand_settings_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "user_hand_settings_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_promo_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    number_of_days = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expire_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_promo_codes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_codes",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "influencer_codes_code_key",
                table: "influencer_codes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "influencer_codes_normalized_code_key",
                table: "influencer_codes",
                column: "normalized_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_influencer_codes_category_id",
                table: "influencer_codes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "influencer_codes_categories_category_name_key",
                table: "influencer_codes_categories",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_login_with_qydha_requests_user_id",
                table: "login_with_qydha_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_users_link_notification_id",
                table: "notifications_users_link",
                column: "notification_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_users_link_user_id",
                table: "notifications_users_link",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_authentication_requests_UserId",
                table: "phone_authentication_requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_user_id",
                table: "purchases",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_update_email_requests_user_id",
                table: "update_email_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_update_phone_requests_user_id",
                table: "update_phone_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_promo_codes_user_id",
                table: "user_promo_codes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_normalized_email_key",
                table: "users",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_normalized_username_key",
                table: "users",
                column: "normalized_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_phone_key",
                table: "users",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_username_key",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "app_assets");

            migrationBuilder.DropTable(
                name: "influencer_codes");

            migrationBuilder.DropTable(
                name: "login_with_qydha_requests");

            migrationBuilder.DropTable(
                name: "notifications_users_link");

            migrationBuilder.DropTable(
                name: "phone_authentication_requests");

            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "registration_otp_request");

            migrationBuilder.DropTable(
                name: "update_email_requests");

            migrationBuilder.DropTable(
                name: "update_phone_requests");

            migrationBuilder.DropTable(
                name: "user_baloot_settings");

            migrationBuilder.DropTable(
                name: "user_general_settings");

            migrationBuilder.DropTable(
                name: "user_hand_settings");

            migrationBuilder.DropTable(
                name: "user_promo_codes");

            migrationBuilder.DropTable(
                name: "influencer_codes_categories");

            migrationBuilder.DropTable(
                name: "notifications_data");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
