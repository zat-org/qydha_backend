namespace Qydha.Infrastructure.Repositories;

public class AppAssetsRepo(IDbConnection dbConnection, ILogger<AppAssetsRepo> logger) : GenericRepository<AppAsset>(dbConnection, logger), IAppAssetsRepo
{
    public async Task<Result<BookAsset>> GetBalootBookAssetData()
    {
        return (await GetByUniquePropAsync(nameof(AppAsset.AssetKey), "baloot_book"))
        .OnSuccess((asset) =>
        {
            BookAsset bookAsset = JsonConvert.DeserializeObject<BookAsset>(asset.AssetData) ??
                 throw new Exception("baloot_book Deserialization failed book Asset = null");
            return Result.Ok(bookAsset);
        });
    }

    public async Task<Result> UpdateBalootBookAssetData(BookAsset bookAsset)
    {
        string serializedBookData = JsonConvert.SerializeObject(bookAsset);
        return await PatchById("baloot_book", nameof(AppAsset.AssetData), serializedBookData);
    }

    public async Task<Result<PopUpAsset>> GetPopupAssetData()
    {
        return (await GetByUniquePropAsync(nameof(AppAsset.AssetKey), "popup"))
        .OnSuccess((asset) =>
        {
            PopUpAsset popupAsset = JsonConvert.DeserializeObject<PopUpAsset>(asset.AssetData) ??
                 throw new Exception("Popup Deserialization failed book Asset = null");
            return Result.Ok(popupAsset);
        });
    }

    public async Task<Result> UpdatePopupAssetData(PopUpAsset popupAsset)
    {
        string serializedPopupAsset = JsonConvert.SerializeObject(popupAsset);
        return await PatchById("popup", nameof(AppAsset.AssetData), serializedPopupAsset);
    }
}
