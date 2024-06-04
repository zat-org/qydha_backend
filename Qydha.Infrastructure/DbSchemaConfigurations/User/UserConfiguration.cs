namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
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
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_on");
        builder.Property(e => e.Email)
            .HasMaxLength(200)
            .HasColumnName("email");
        builder.Property(e => e.ExpireDate)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("expire_date");
        builder.Property(e => e.FCMToken)
            .HasMaxLength(200)
            .HasColumnName("fcm_token");

        builder.Property(e => e.IsAnonymous)
            .HasDefaultValue(false)
            .HasColumnName("is_anonymous");
        builder.Property(e => e.IsEmailConfirmed)
            .HasDefaultValue(false)
            .HasColumnName("is_email_confirmed");
        builder.Property(e => e.IsPhoneConfirmed)
            .HasDefaultValue(false)
            .HasColumnName("is_phone_confirmed");
        builder.Property(e => e.LastLogin)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("last_login");
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .HasColumnName("name");
        builder.Property(e => e.NormalizedEmail)
            .HasMaxLength(200)
            .HasColumnName("normalized_email");
        builder.Property(e => e.NormalizedUsername)
            .HasMaxLength(100)
            .HasColumnName("normalized_username");
        builder.Property(e => e.PasswordHash)
            .HasColumnType("character varying")
            .HasColumnName("password_hash");
        builder.Property(e => e.Phone)
            .HasMaxLength(30)
            .HasColumnName("phone");
        builder.Property(e => e.Username)
            .HasMaxLength(100)
            .HasColumnName("username");
    }
}