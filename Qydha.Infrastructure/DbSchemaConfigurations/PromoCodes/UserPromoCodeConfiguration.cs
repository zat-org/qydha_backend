
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class UserPromoCodeConfiguration : IEntityTypeConfiguration<UserPromoCode>
{
    public void Configure(EntityTypeBuilder<UserPromoCode> entity)
    {
        entity.HasKey(e => e.Id).HasName("user_promo_codes_pkey");

        entity.ToTable("user_promo_codes");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.Code)
            .HasMaxLength(50)
            .HasColumnName("code");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("created_at");
        entity.Property(e => e.ExpireAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("expire_at");
        entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");
        entity.Property(e => e.UsedAt)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("used_at");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany(p => p.UserPromoCodes)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_user_codes");
    }
}