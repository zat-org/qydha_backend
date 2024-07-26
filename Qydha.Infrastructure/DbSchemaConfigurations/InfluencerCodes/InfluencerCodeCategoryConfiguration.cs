
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class InfluencerCodeCategoryConfiguration : IEntityTypeConfiguration<InfluencerCodeCategory>
{
    public void Configure(EntityTypeBuilder<InfluencerCodeCategory> entity)
    {
        entity.HasKey(e => e.Id).HasName("influencer_codes_categories_pkey");

        entity.ToTable("influencer_codes_categories");

        entity.HasIndex(e => e.CategoryName, "influencer_codes_categories_category_name_key").IsUnique();

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.CategoryName)
            .HasMaxLength(100)
            .HasColumnName("category_name");
        entity.Property(e => e.MaxCodesPerUserInGroup)
            .HasDefaultValue(1)
            .HasColumnName("max_codes_per_user_in_group");
    }
}