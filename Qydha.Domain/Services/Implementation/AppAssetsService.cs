namespace Qydha.Domain.Services.Implementation;

public class AppAssetsService(IAppAssetsRepo appAssetsRepo, IFileService fileService, ILogger<AppAssetsService> logger, IOptions<BookSettings> bookOptions, IOptions<NotificationImageSettings> notificationImageOptions) : IAppAssetsService
{
    private readonly IAppAssetsRepo _appAssetsRepo = appAssetsRepo;
    private readonly IFileService _fileService = fileService;
    private readonly ILogger<AppAssetsService> _logger = logger;
    private readonly BookSettings bookOptions = bookOptions.Value;
    private readonly NotificationImageSettings _notificationImageOptions = notificationImageOptions.Value;

    public async Task<Result<PopUpAsset>> GetPopupAssetData() => await _appAssetsRepo.GetPopupAssetData();
    public async Task<Result<BookAsset>> GetBalootBookData() => await _appAssetsRepo.GetBalootBookAssetData();
    public async Task<Result> UpdatePopupData(PopUpAsset popupAsset)
    {
        if (popupAsset.Show && popupAsset.Image is null)
            return Result.Fail(new InvalidBodyInputError("لا يمكن تحويل حالة الاعلان الي  ظاهر وهو بدون صورة"));
        else
            return await _appAssetsRepo.UpdatePopupAssetData(popupAsset);
    }

    public async Task<Result<BookAsset>> UpdateBalootBookData(IFormFile bookFile)
    {
        return (await _appAssetsRepo.GetBalootBookAssetData())
        .OnSuccessAsync(async (bookAsset) => (await _fileService.UploadFile(bookOptions.FolderPath, bookFile))
            .ToResult((fileData) => (bookAsset, fileData)))
        .OnSuccessAsync(async (tuple) =>
        {
            if (tuple.bookAsset.Path != null)
                await _fileService.DeleteFile(tuple.bookAsset.Path);
            return Result.Ok(tuple.fileData);
        })
        .OnSuccessAsync(async (fileData) =>
        {
            var bookAsset = new BookAsset()
            {
                LastUpdateAt = DateTimeOffset.UtcNow,
                Path = fileData.Path,
                Url = fileData.Url
            };
            return (await _appAssetsRepo.UpdateBalootBookAssetData(bookAsset)).ToResult(bookAsset);
        });
    }

    public async Task<Result<PopUpAsset>> UpdatePopupImage(IFormFile imageFile)
    {
        return (await _appAssetsRepo.GetPopupAssetData())
        .OnSuccessAsync(async (popupAsset) => (await _fileService.UploadFile(_notificationImageOptions.FolderPath, imageFile))
                .ToResult((fileData) => (popupAsset, fileData)))
        .OnSuccessAsync(async (tuple) =>
        {
            if (tuple.popupAsset.Image != null)
                await _fileService.DeleteFile(tuple.popupAsset.Image.Path);
            tuple.popupAsset.Image = tuple.fileData;
            return Result.Ok(tuple.popupAsset);
        })
        .OnSuccessAsync(async (popupAsset) =>
            (await _appAssetsRepo.UpdatePopupAssetData(popupAsset)).ToResult(popupAsset));
    }
}
