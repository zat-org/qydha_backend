
namespace Qydha.Infrastructure.DbSchemaConfiguration;
public class AppAssetsConfiguration : IEntityTypeConfiguration<AppAsset>
{
    public void Configure(EntityTypeBuilder<AppAsset> entity)
    {

        var AppAssets = new AppAsset[] {
            new(){
                AssetKey = "baloot_book",
                AssetData = "{}"
                },
            new(){
                AssetKey = "popup",
                AssetData = "{}"
            }
        };

        entity.HasData(AppAssets);
        entity.HasKey(e => e.AssetKey).HasName("app_assets_pkey");

        entity.ToTable("app_assets");

        entity.Property(e => e.AssetKey)
            .HasMaxLength(100)
            .HasColumnName("asset_key");
        entity.Property(e => e.AssetData)
            .HasColumnType("jsonb")
            .HasColumnName("asset_data");
    }
}