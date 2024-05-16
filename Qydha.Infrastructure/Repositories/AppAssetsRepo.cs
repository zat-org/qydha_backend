namespace Qydha.Infrastructure.Repositories;

public class AppAssetsRepo(QydhaContext dbContext, ILogger<AppAssetsRepo> logger) : IAppAssetsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<AppAssetsRepo> _logger = logger;
    private const string BalootBookKey = "baloot_book";
    private const string PopupKey = "popup";
    public async Task<Result<BookAsset>> GetBalootBookAssetData()
    {
        var asset = await _dbCtx.AppAssets.FirstOrDefaultAsync((elm) => elm.AssetKey == BalootBookKey);
        if (asset is not null && asset.AssetData is not null)
        {
            BookAsset? bookAsset = JsonConvert.DeserializeObject<BookAsset>(asset.AssetData);
            if (bookAsset == null)
            {
                _logger.LogCritical("can't Deserialization baloot book asset with key : {assetKey}", BalootBookKey);
                return Result.Fail(new EntityNotFoundError<string>(BalootBookKey, nameof(BookAsset), ErrorType.AssetNotFound));
            }
            return Result.Ok(bookAsset);
        }
        return Result.Fail(new EntityNotFoundError<string>(BalootBookKey, nameof(BookAsset), ErrorType.AssetNotFound));
    }

    public async Task<Result> UpdateBalootBookAssetData(BookAsset bookAsset)
    {
        string serializedBookData = JsonConvert.SerializeObject(bookAsset);
        var affected = await _dbCtx.AppAssets.Where(asset => asset.AssetKey == BalootBookKey).ExecuteUpdateAsync(
            setter => setter.SetProperty(asset => asset.AssetData, serializedBookData)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<string>(BalootBookKey, nameof(BookAsset), ErrorType.AssetNotFound));

    }

    public async Task<Result<PopUpAsset>> GetPopupAssetData()
    {
        var asset = await _dbCtx.AppAssets.FirstOrDefaultAsync((elm) => elm.AssetKey == PopupKey);
        if (asset is not null && asset.AssetData is not null)
        {
            PopUpAsset? popupAsset = JsonConvert.DeserializeObject<PopUpAsset>(asset.AssetData);
            if (popupAsset == null)
            {
                _logger.LogCritical("can't Deserialization Popup asset with key : {assetKey}", PopupKey);
                return Result.Fail(new EntityNotFoundError<string>(PopupKey, nameof(PopUpAsset), ErrorType.AssetNotFound));
            }
            return Result.Ok(popupAsset);
        }
        return Result.Fail(new EntityNotFoundError<string>(PopupKey, nameof(PopUpAsset), ErrorType.AssetNotFound));
    }

    public async Task<Result> UpdatePopupAssetData(PopUpAsset popupAsset)
    {
        string serializedPopupAsset = JsonConvert.SerializeObject(popupAsset);
        var affected = await _dbCtx.AppAssets.Where(asset => asset.AssetKey == PopupKey).ExecuteUpdateAsync(
            setter => setter.SetProperty(asset => asset.AssetData, serializedPopupAsset)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<string>(PopupKey, nameof(PopUpAsset), ErrorType.AssetNotFound));
    }
}
