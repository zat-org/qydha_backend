
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> entity)
    {
        entity.HasKey(e => e.Id).HasName("purchases_pkey");

        entity.ToTable("purchases");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.IAPHubPurchaseId)
            .HasMaxLength(40)
            .HasColumnName("iaphub_purchase_id");
        entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");
        entity.Property(e => e.ProductSku)
            .HasMaxLength(15)
            .HasColumnName("productsku");
        entity.Property(e => e.PurchaseDate)
            .HasColumnType("timestamp with time zone").HasConversion(ConfigurationUtils.DateTimeOffsetValueConverter)
            .HasColumnName("purchase_date");
        entity.Property(e => e.Type)
            .HasMaxLength(10)
            .HasColumnName("type");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany(p => p.Purchases)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_user")
            .OnDelete(DeleteBehavior.Cascade);
    }
}