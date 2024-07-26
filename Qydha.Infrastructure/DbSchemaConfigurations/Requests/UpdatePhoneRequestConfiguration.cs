
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UpdatePhoneRequestConfiguration : IEntityTypeConfiguration<UpdatePhoneRequest>
{
    public void Configure(EntityTypeBuilder<UpdatePhoneRequest> entity)
    {
        entity.HasKey(e => e.Id).HasName("update_phone_requests_pkey");

        entity.ToTable("update_phone_requests");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("created_on");
        entity.Property(e => e.OTP)
            .HasMaxLength(6)
            .HasColumnName("otp");
        entity.Property(e => e.Phone)
            .HasMaxLength(30)
            .HasColumnName("phone");
        entity.Property(e => e.UserId).HasColumnName("user_id");
    }
}