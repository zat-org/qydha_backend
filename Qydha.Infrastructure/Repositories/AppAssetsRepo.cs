namespace Qydha.Infrastructure.Repositories;

public class AppAssetsRepo(QydhaContext dbContext, ILogger<AppAssetsRepo> logger) :IAppAssetsRepo
{
    private readonly QydhaContext  _dbCtx = dbContext;
    private readonly ILogger<AppAssetsRepo> _logger = logger;
    public async Task<Result<BookAsset>> GetBalootBookAssetData()
    {
        var asset = await _dbCtx.AppAssets.FirstOrDefaultAsync((elm) => elm.AssetKey == "baloot_book");
        if(asset is not null && asset.AssetData is not null){
            BookAsset bookAsset = JsonConvert.DeserializeObject<BookAsset>(asset.AssetData) ??
                //! TODO :: check this exception position and handling and logging
                throw new Exception("baloot_book Deserialization failed book Asset = null");
            return Result.Ok(bookAsset);
        }else {
            return Result.Fail<BookAsset>(new(){
                Code= ErrorType.AssetNotFound,
                Message = "Baloot Book Asset Not Found"
            });
        }
    }

    public async Task<Result> UpdateBalootBookAssetData(BookAsset bookAsset)
    {
        string serializedBookData = JsonConvert.SerializeObject(bookAsset);
        var affected = await _dbCtx.AppAssets.Where(asset => asset.AssetKey == "baloot_book").ExecuteUpdateAsync(
            setter => setter.SetProperty(asset=> asset.AssetData , serializedBookData)
        );
        return affected == 1 ? 
            Result.Ok() :
            Result.Fail(new(){
                Code= ErrorType.AssetNotFound,
                Message = "Baloot Book Asset Not Found"
            });
    }

    public async Task<Result<PopUpAsset>> GetPopupAssetData()
    {
        var asset = await _dbCtx.AppAssets.FirstOrDefaultAsync((elm) => elm.AssetKey == "popup");
        if(asset is not null && asset.AssetData is not null){
            PopUpAsset popupAsset = JsonConvert.DeserializeObject<PopUpAsset>(asset.AssetData) ??
                //! TODO :: check this exception position and handling and logging
                throw new Exception("Popup Asset Deserialization failed Popup Asset = null ");
            return Result.Ok(popupAsset);
        } else {
            return Result.Fail<PopUpAsset>(new(){
                Code= ErrorType.AssetNotFound,
                Message = "Popup Asset Not Found"
            });
        }
    }

    public async Task<Result> UpdatePopupAssetData(PopUpAsset popupAsset)
    {
        string serializedPopupAsset = JsonConvert.SerializeObject(popupAsset);
        var affected = await _dbCtx.AppAssets.Where(asset => asset.AssetKey == "popup").ExecuteUpdateAsync(
            setter => setter.SetProperty(asset=> asset.AssetData , serializedPopupAsset)
        );
        return affected == 1 ? 
            Result.Ok() :
            Result.Fail(new(){
                Code= ErrorType.AssetNotFound,
                Message = "popup Asset Not Found"
            });
    }
}
