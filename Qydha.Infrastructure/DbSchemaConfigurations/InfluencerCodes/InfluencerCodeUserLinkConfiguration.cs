
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class InfluencerCodeUserLinkConfiguration : IEntityTypeConfiguration<InfluencerCodeUserLink>
{
    public void Configure(EntityTypeBuilder<InfluencerCodeUserLink> entity)
    {
        entity.HasKey(e => new { e.InfluencerCodeId, e.UserId }).HasName("influencer_code_users_link_pkey");

        entity.ToTable("influencer_code_users_link");

        entity.Property(e => e.UsedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("used_at");
        entity.Property(e => e.NumberOfDays)
            .HasColumnName("number_of_days");

        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.InfluencerCodeId).HasColumnName("influencer_code_id");

        entity.HasOne(d => d.User).WithMany(p => p.InfluencerCodes)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_user_at_influencer_code_user_link_table");

        entity.HasOne(d => d.InfluencerCode).WithMany(p => p.Users)
            .HasForeignKey(d => d.InfluencerCodeId)
            .HasConstraintName("fk_influencer_code_at_influencer_code_link_table");
    }
}