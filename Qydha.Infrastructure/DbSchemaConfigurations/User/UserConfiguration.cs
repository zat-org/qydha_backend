using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        var adminId = new Guid("d2705466-4304-4830-b48a-3e44e031927e");
        var superAdmin = new User(
            id: adminId,
            username: "Admin",
            phone: "+201555330346",
            roles: [UserRoles.SuperAdmin, UserRoles.User],
            createdAt: DateTimeOffset.Parse("2023-11-01T00:00:00.000000Z"),
            passwordHash: "$2a$11$V0A5.EYwXlFUjK3RIis3...A9rfzUm.mO.88MUYW9.uHSZLjURNsC")
        {
        };

        builder.HasData(superAdmin);

        builder.HasKey(e => e.Id).HasName("users_pkey");

        builder.ToTable("users");

        builder.HasIndex(e => e.Email, "users_email_key").IsUnique();

        builder.HasIndex(e => e.NormalizedEmail, "users_normalized_email_key").IsUnique();

        builder.HasIndex(e => e.NormalizedUsername, "users_normalized_username_key").IsUnique();

        builder.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

        builder.HasIndex(e => e.Username, "users_username_key").IsUnique();

        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        builder.Property(e => e.AvatarPath)
            .HasColumnType("character varying")
            .HasColumnName("avatar_path");
        builder.Property(e => e.AvatarUrl)
            .HasColumnType("character varying")
            .HasColumnName("avatar_url");
        builder.Property(e => e.BirthDate)
            .HasColumnType("DATE")
            .HasColumnName("birth_date");
        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("created_on");
        builder.Property(e => e.Email)
            .HasMaxLength(200)
            .HasColumnName("email");
        builder.Property(e => e.ExpireDate)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("expire_date");
        builder.Property(e => e.FCMToken)
            .IsRequired(false)
            .HasMaxLength(200)
            .HasColumnName("fcm_token");

        builder.Property(e => e.LastLogin)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("last_login");
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(e => e.Roles)
            .HasColumnName("roles");

        builder.Property(e => e.NormalizedEmail)
            .HasMaxLength(200)
            .HasColumnName("normalized_email")
            .HasComputedColumnSql("UPPER(email)", true);


        builder.Property(e => e.NormalizedUsername)
            .HasMaxLength(100)
            .HasColumnName("normalized_username")
            .HasComputedColumnSql("UPPER(username)", true);
        builder.Property(e => e.PasswordHash)
            .HasColumnType("character varying")
            .HasColumnName("password_hash");
        builder.Property(e => e.Phone)
            .HasMaxLength(30)
            .HasColumnName("phone");
        builder.Property(e => e.Username)
            .HasMaxLength(100)
            .HasColumnName("username");
        builder.OwnsOne(d => d.UserBalootSettings, entity =>
        {
            entity.HasData(new { UserId = adminId });

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.HasKey(e => e.UserId);
            entity.ToTable("user_baloot_settings");
            entity.Property(e => e.IsAdvancedRecording)
                .HasDefaultValue(false)
                .HasColumnName("is_advanced_recording");
            entity.Property(e => e.IsCommentsSoundEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_comments_sound_enabled");
            entity.Property(e => e.IsFlipped)
                .HasDefaultValue(false)
                .HasColumnName("is_flipped");
            entity.Property(e => e.IsNumbersSoundEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_numbers_sound_enabled");
            entity.Property(e => e.IsSakkahMashdodahMode)
                .HasDefaultValue(false)
                .HasColumnName("is_sakkah_mashdodah_mode");
            entity.Property(e => e.ShowWhoWonDialogOnDraw)
                .HasDefaultValue(false)
                .HasColumnName("show_who_won_dialog_on_draw");
            entity.Property(e => e.IsEkakShown)
                .HasDefaultValue(false)
                .HasColumnName("is_ekak_shown");
            entity.Property(e => e.IsAklatShown)
                .HasDefaultValue(false)
                .HasColumnName("is_aklat_shown");
            entity.Property(e => e.SakkasCount)
                .HasDefaultValue(1)
                .HasColumnName("sakkas_count");
        });
        builder.OwnsOne(d => d.UserGeneralSettings, entity =>
        {
            entity.HasData(new UserGeneralSettings() { UserId = adminId, PlayersNames = [], TeamsNames = [] });

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.HasKey(e => e.UserId);
            entity.ToTable("user_general_settings");
            entity.Property(e => e.EnableVibration)
                .HasDefaultValue(true)
                .HasColumnName("enable_vibration");
            entity.Property(e => e.PlayersNames)
                .HasColumnName("players_names");
            entity.Property(e => e.TeamsNames)
                .HasColumnName("teams_names");
        });
        builder.OwnsOne(d => d.UserHandSettings, entity =>
        {
            entity.HasData(new { UserId = adminId });

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.HasKey(e => e.UserId);
            entity.ToTable("user_hand_settings");
            entity.Property(e => e.MaxLimit)
                .HasDefaultValue(0)
                .HasColumnName("max_limit");
            entity.Property(e => e.PlayersCountInTeam)
                .HasDefaultValue(2)
                .HasColumnName("players_count_in_team");
            entity.Property(e => e.RoundsCount)
                .HasDefaultValue(7)
                .HasColumnName("rounds_count");
            entity.Property(e => e.TakweeshPoints)
                .HasDefaultValue(100)
                .HasColumnName("takweesh_points");
            entity.Property(e => e.TeamsCount)
                .HasDefaultValue(2)
                .HasColumnName("teams_count");
            entity.Property(e => e.WinUsingZat)
                .HasDefaultValue(false)
                .HasColumnName("win_using_zat");
        });
    }
}