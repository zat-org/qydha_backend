namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class RegistrationOTPRequestConfiguration : IEntityTypeConfiguration<RegistrationOTPRequest>
{
    public void Configure(EntityTypeBuilder<RegistrationOTPRequest> entity)
    {
        entity.HasKey(e => e.Id).HasName("registration_otp_request_pkey");

        entity.ToTable("registration_otp_request");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("created_on");
        entity.Property(e => e.FCMToken)
            .HasMaxLength(200)
            .HasColumnName("fcm_token");
        entity.Property(e => e.OTP)
            .HasMaxLength(6)
            .HasColumnName("otp");
        entity.Property(e => e.PasswordHash)
            .HasColumnType("character varying")
            .HasColumnName("password_hash");
        entity.Property(e => e.Phone)
            .HasMaxLength(30)
            .HasColumnName("phone");
        entity.Property(e => e.Username)
            .HasMaxLength(100)
            .HasColumnName("username");

    }
}