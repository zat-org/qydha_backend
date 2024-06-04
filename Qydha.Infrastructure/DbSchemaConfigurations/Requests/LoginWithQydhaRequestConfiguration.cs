namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class LoginWithQydhaRequestConfiguration : IEntityTypeConfiguration<LoginWithQydhaRequest>
{
    public void Configure(EntityTypeBuilder<LoginWithQydhaRequest> entity)
    {
        entity.HasKey(e => e.Id).HasName("login_with_qydha_requests_pkey");

        entity.ToTable("login_with_qydha_requests");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at");
        entity.Property(e => e.Otp)
            .HasMaxLength(6)
            .HasColumnName("otp");
        entity.Property(e => e.UsedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("used_at");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany(p => p.LoginWithQydhaRequests)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_user_id");
    }
}