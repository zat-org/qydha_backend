﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Qydha.Infrastructure;

#nullable disable

namespace Qydha.Api.Migrations
{
    [DbContext(typeof(QydhaContext))]
    partial class QydhaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Qydha.Domain.Entities.AdminUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("NormalizedUsername")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("normalized_username");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password_hash");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)")
                        .HasColumnName("role");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("admins_pkey");

                    b.HasIndex(new[] { "NormalizedUsername" }, "admins_normalized_username_key")
                        .IsUnique();

                    b.HasIndex(new[] { "Username" }, "admins_username_key")
                        .IsUnique();

                    b.ToTable("admins", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.AppAsset", b =>
                {
                    b.Property<string>("AssetKey")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("asset_key");

                    b.Property<string>("AssetData")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("asset_data");

                    b.HasKey("AssetKey")
                        .HasName("app_assets_pkey");

                    b.ToTable("app_assets", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.InfluencerCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expire_at");

                    b.Property<int>("MaxInfluencedUsersCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("max_influenced_users_count");

                    b.Property<string>("NormalizedCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("normalized_code");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("integer")
                        .HasColumnName("number_of_days");

                    b.HasKey("Id")
                        .HasName("influencer_codes_pkey");

                    b.HasIndex("CategoryId");

                    b.HasIndex(new[] { "Code" }, "influencer_codes_code_key")
                        .IsUnique();

                    b.HasIndex(new[] { "NormalizedCode" }, "influencer_codes_normalized_code_key")
                        .IsUnique();

                    b.ToTable("influencer_codes", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.InfluencerCodeCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("category_name");

                    b.Property<int>("MaxCodesPerUserInGroup")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("max_codes_per_user_in_group");

                    b.HasKey("Id")
                        .HasName("influencer_codes_categories_pkey");

                    b.HasIndex(new[] { "CategoryName" }, "influencer_codes_categories_category_name_key")
                        .IsUnique();

                    b.ToTable("influencer_codes_categories", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.LoginWithQydhaRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Otp")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("otp");

                    b.Property<DateTime?>("UsedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("used_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("login_with_qydha_requests_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("login_with_qydha_requests", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.NotificationData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ActionPath")
                        .IsRequired()
                        .HasMaxLength(350)
                        .HasColumnType("character varying(350)")
                        .HasColumnName("action_path");

                    b.Property<int>("ActionType")
                        .HasColumnType("integer")
                        .HasColumnName("action_type");

                    b.Property<int>("AnonymousClicks")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("anonymous_clicks");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("description");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasColumnName("payload")
                        .HasDefaultValueSql("'{}'::jsonb");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("title");

                    b.Property<int>("Visibility")
                        .HasColumnType("integer")
                        .HasColumnName("visibility");

                    b.HasKey("Id")
                        .HasName("notifications_data_pkey");

                    b.ToTable("notifications_data", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.NotificationUserLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("NotificationId")
                        .HasColumnType("integer")
                        .HasColumnName("notification_id");

                    b.Property<DateTime?>("ReadAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("read_at");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("sent_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("notifications_users_link_pkey");

                    b.HasIndex("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("notifications_users_link", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.PhoneAuthenticationRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Otp")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("otp");

                    b.Property<DateTime?>("UsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("phone_authentication_requests_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("phone_authentication_requests", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.Purchase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("IAPHubPurchaseId")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("iaphub_purchase_id");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("integer")
                        .HasColumnName("number_of_days");

                    b.Property<string>("ProductSku")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("productsku");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("purchase_date");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("type");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("purchases_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("purchases", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.RegistrationOTPRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("FCMToken")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("fcm_token");

                    b.Property<string>("OTP")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("otp");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("password_hash");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("phone");

                    b.Property<DateTime?>("UsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("registration_otp_request_pkey");

                    b.ToTable("registration_otp_request", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UpdateEmailRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("email");

                    b.Property<string>("OTP")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("otp");

                    b.Property<DateTime?>("UsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("update_email_requests_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("update_email_requests", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UpdatePhoneRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("OTP")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("otp");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("phone");

                    b.Property<DateTime?>("UsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("update_phone_requests_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("update_phone_requests", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("AvatarPath")
                        .HasColumnType("character varying")
                        .HasColumnName("avatar_path");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("character varying")
                        .HasColumnName("avatar_url");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_date");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("ExpireDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expire_date");

                    b.Property<string>("FCMToken")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("fcm_token");

                    b.Property<int>("FreeSubscriptionUsed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("free_subscription_used");

                    b.Property<bool>("IsAnonymous")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_anonymous");

                    b.Property<bool>("IsEmailConfirmed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_email_confirmed");

                    b.Property<bool>("IsPhoneConfirmed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_phone_confirmed");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_login");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUsername")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("normalized_username");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("password_hash");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("phone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("users_pkey");

                    b.HasIndex(new[] { "Email" }, "users_email_key")
                        .IsUnique();

                    b.HasIndex(new[] { "NormalizedEmail" }, "users_normalized_email_key")
                        .IsUnique();

                    b.HasIndex(new[] { "NormalizedUsername" }, "users_normalized_username_key")
                        .IsUnique();

                    b.HasIndex(new[] { "Phone" }, "users_phone_key")
                        .IsUnique();

                    b.HasIndex(new[] { "Username" }, "users_username_key")
                        .IsUnique();

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserBalootSettings", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<bool>("IsAdvancedRecording")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_advanced_recording");

                    b.Property<bool>("IsCommentsSoundEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("is_comments_sound_enabled");

                    b.Property<bool>("IsFlipped")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_flipped");

                    b.Property<bool>("IsNumbersSoundEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("is_numbers_sound_enabled");

                    b.Property<bool>("IsSakkahMashdodahMode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_sakkah_mashdodah_mode");

                    b.Property<bool>("ShowWhoWonDialogOnDraw")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("show_who_won_dialog_on_draw");

                    b.HasKey("UserId")
                        .HasName("user_baloot_settings_pkey");

                    b.ToTable("user_baloot_settings", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserGeneralSettings", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<bool>("EnableVibration")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("enable_vibration");

                    b.Property<string>("PlayersNames")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasColumnName("players_names")
                        .HasDefaultValueSql("'[]'::jsonb");

                    b.Property<string>("TeamsNames")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasColumnName("teams_names")
                        .HasDefaultValueSql("'[]'::jsonb");

                    b.HasKey("UserId")
                        .HasName("user_general_settings_pkey");

                    b.ToTable("user_general_settings", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserHandSettings", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<int>("MaxLimit")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("max_limit");

                    b.Property<int>("PlayersCountInTeam")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(2)
                        .HasColumnName("players_count_in_team");

                    b.Property<int>("RoundsCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(7)
                        .HasColumnName("rounds_count");

                    b.Property<int>("TakweeshPoints")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(100)
                        .HasColumnName("takweesh_points");

                    b.Property<int>("TeamsCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(2)
                        .HasColumnName("teams_count");

                    b.Property<bool>("WinUsingZat")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("win_using_zat");

                    b.HasKey("UserId")
                        .HasName("user_hand_settings_pkey");

                    b.ToTable("user_hand_settings", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserPromoCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("ExpireAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expire_at");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("integer")
                        .HasColumnName("number_of_days");

                    b.Property<DateTime?>("UsedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("used_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("user_promo_codes_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("user_promo_codes", (string)null);
                });

            modelBuilder.Entity("Qydha.Domain.Entities.InfluencerCode", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.InfluencerCodeCategory", "Category")
                        .WithMany("InfluencerCodes")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("fk_influencer_code_categories");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.LoginWithQydhaRequest", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithMany("LoginWithQydhaRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.NotificationUserLink", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.NotificationData", "Notification")
                        .WithMany("NotificationUserLinks")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_notification_at_notification_link_table");

                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithMany("NotificationUserLinks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_at_notification_link_table");

                    b.Navigation("Notification");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.PhoneAuthenticationRequest", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", null)
                        .WithMany("PhoneAuthenticationRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Qydha.Domain.Entities.Purchase", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithMany("Purchases")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UpdateEmailRequest", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", null)
                        .WithMany("UpdateEmailRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UpdatePhoneRequest", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", null)
                        .WithMany("UpdatePhoneRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserBalootSettings", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithOne("UserBalootSettings")
                        .HasForeignKey("Qydha.Domain.Entities.UserBalootSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("user_baloot_settings_user_id_fkey");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserGeneralSettings", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithOne("UserGeneralSettings")
                        .HasForeignKey("Qydha.Domain.Entities.UserGeneralSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("user_general_settings_user_id_fkey");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserHandSettings", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithOne("UserHandSettings")
                        .HasForeignKey("Qydha.Domain.Entities.UserHandSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("user_hand_settings_user_id_fkey");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.UserPromoCode", b =>
                {
                    b.HasOne("Qydha.Domain.Entities.User", "User")
                        .WithMany("UserPromoCodes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_codes");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.InfluencerCodeCategory", b =>
                {
                    b.Navigation("InfluencerCodes");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.NotificationData", b =>
                {
                    b.Navigation("NotificationUserLinks");
                });

            modelBuilder.Entity("Qydha.Domain.Entities.User", b =>
                {
                    b.Navigation("LoginWithQydhaRequests");

                    b.Navigation("NotificationUserLinks");

                    b.Navigation("PhoneAuthenticationRequests");

                    b.Navigation("Purchases");

                    b.Navigation("UpdateEmailRequests");

                    b.Navigation("UpdatePhoneRequests");

                    b.Navigation("UserBalootSettings")
                        .IsRequired();

                    b.Navigation("UserGeneralSettings")
                        .IsRequired();

                    b.Navigation("UserHandSettings")
                        .IsRequired();

                    b.Navigation("UserPromoCodes");
                });
#pragma warning restore 612, 618
        }
    }
}
