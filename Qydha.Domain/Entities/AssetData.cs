namespace Qydha.Domain.Entities;
[Table("app_assets")]

public class AppAsset : DbEntity<AppAsset>
{
    [Key]
    [Column("asset_key")]
    public string AssetKey { get; set; } = null!;

    [Column("asset_data")]
    [JsonField]
    public string AssetData { get; set; } = null!;

}
