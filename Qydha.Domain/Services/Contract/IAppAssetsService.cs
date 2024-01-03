namespace Qydha.Domain.Services.Contracts;

public interface IAppAssetsService
{
    Task<Result<BookAsset>> GetBalootBookData();
    Task<Result<BookAsset>> UpdateBalootBookData(IFormFile bookFile);
    Task<Result<PopUpAsset>> GetPopupAssetData();
    Task<Result> UpdatePopupData(PopUpAsset popupAsset);
    Task<Result<PopUpAsset>> UpdatePopupImage(IFormFile imageFile);
}
