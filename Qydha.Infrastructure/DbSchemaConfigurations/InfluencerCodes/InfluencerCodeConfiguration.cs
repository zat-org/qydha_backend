
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class InfluencerCodeConfiguration : IEntityTypeConfiguration<InfluencerCode>
{
    public void Configure(EntityTypeBuilder<InfluencerCode> entity)
    {
        entity.HasKey(e => e.Id).HasName("influencer_codes_pkey");

        entity.ToTable("influencer_codes");

        entity.HasIndex(e => e.Code, "influencer_codes_code_key").IsUnique();

        entity.HasIndex(e => e.NormalizedCode, "influencer_codes_normalized_code_key").IsUnique();

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.CategoryId).HasColumnName("category_id");
        entity.Property(e => e.Code)
            .HasMaxLength(100)
            .HasColumnName("code");
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at");
        entity.Property(e => e.ExpireAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("expire_at");
        entity.Property(e => e.MaxInfluencedUsersCount)
            .HasDefaultValue(0)
            .HasColumnName("max_influenced_users_count");
        entity.Property(e => e.NormalizedCode)
            .HasMaxLength(100)
            .HasColumnName("normalized_code");
        entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");

        entity.HasOne(d => d.Category).WithMany(p => p.InfluencerCodes)
            .HasForeignKey(d => d.CategoryId)
            .HasConstraintName("fk_influencer_code_categories");
    }
}