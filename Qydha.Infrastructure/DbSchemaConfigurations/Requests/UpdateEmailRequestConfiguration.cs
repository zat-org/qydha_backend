
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UpdateEmailRequestConfiguration : IEntityTypeConfiguration<UpdateEmailRequest>
{
    public void Configure(EntityTypeBuilder<UpdateEmailRequest> entity)
    {
        entity.HasKey(e => e.Id).HasName("update_email_requests_pkey");

        entity.ToTable("update_email_requests");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_on");
        entity.Property(e => e.Email)
            .HasMaxLength(100)
            .HasColumnName("email");
        entity.Property(e => e.OTP)
            .HasMaxLength(6)
            .HasColumnName("otp");
        entity.Property(e => e.UserId).HasColumnName("user_id");
    }
}