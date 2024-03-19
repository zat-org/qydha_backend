namespace Qydha.Domain.Repositories;

public interface IAppAssetsRepo
{
    Task<Result<BookAsset>> GetBalootBookAssetData();
    Task<Result> UpdateBalootBookAssetData(BookAsset bookAsset);
    Task<Result<PopUpAsset>> GetPopupAssetData();
    Task<Result> UpdatePopupAssetData(PopUpAsset popupAsset);
}
