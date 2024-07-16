namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class PhoneAuthenticationRequestConfiguration : IEntityTypeConfiguration<PhoneAuthenticationRequest>
{
    public void Configure(EntityTypeBuilder<PhoneAuthenticationRequest> entity)
    {
        entity.HasKey(e => e.Id).HasName("phone_authentication_requests_pkey");

        entity.ToTable("phone_authentication_requests");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("created_on");
        entity.Property(e => e.Otp)
            .HasMaxLength(6)
            .HasColumnName("otp");
    }
}