namespace Qydha.Domain.Services.Implementation;

public class AppAssetsService(IAppAssetsRepo appAssetsRepo, IFileService fileService, ILogger<AppAssetsService> logger, IOptions<BookSettings> bookOptions, IOptions<NotificationImageSettings> notificationImageOptions) : IAppAssetsService
{
    private readonly IAppAssetsRepo _appAssetsRepo = appAssetsRepo;
    private readonly IFileService _fileService = fileService;
    private readonly ILogger<AppAssetsService> _logger = logger;
    private readonly BookSettings bookOptions = bookOptions.Value;
    private readonly NotificationImageSettings _notificationImageOptions = notificationImageOptions.Value;


    public async Task<Result<BookAsset>> GetBalootBookData() => await _appAssetsRepo.GetBalootBookAssetData();
    public async Task<Result<BookAsset>> UpdateBalootBookData(IFormFile bookFile)
    {
        return (await _appAssetsRepo.GetBalootBookAssetData())
        .OnSuccessAsync(async (bookAsset) => (await _fileService.UploadFile(bookOptions.FolderPath, bookFile))
            .MapTo((fileData) => new Tuple<BookAsset, FileData>(bookAsset, fileData)))
        .OnSuccessAsync(async (tuple) =>
        {
            if (tuple.Item1.Path is not null)
                (await _fileService.DeleteFile(tuple.Item1.Path)).OnFailure(() =>
                {
                    //! Handle Error On Delete File 
                    _logger.LogError("Can't delete the old book with the path : {path}", tuple.Item1.Path);
                });
            return Result.Ok(tuple.Item2);
        })
        .OnSuccessAsync(async (fileData) =>
        {
            var bookAsset = new BookAsset()
            {
                LastUpdateAt = DateTimeOffset.UtcNow,
                Path = fileData.Path,
                Url = fileData.Url
            };
            return (await _appAssetsRepo.UpdateBalootBookAssetData(bookAsset)).MapTo(bookAsset);
        });
    }
    public async Task<Result<PopUpAsset>> GetPopupAssetData() => await _appAssetsRepo.GetPopupAssetData();

    public async Task<Result> UpdatePopupData(PopUpAsset popupAsset) => await _appAssetsRepo.UpdatePopupAssetData(popupAsset);

    public async Task<Result<PopUpAsset>> UpdatePopupImage(IFormFile imageFile)
    {
        return (await _appAssetsRepo.GetPopupAssetData())
        .OnSuccessAsync(async (popupAsset) => (await _fileService.UploadFile(_notificationImageOptions.FolderPath, imageFile))
                .MapTo((fileData) => new Tuple<PopUpAsset, FileData>(popupAsset, fileData)))
        .OnSuccessAsync<Tuple<PopUpAsset, FileData>>(async (tuple) =>
        {
            if (tuple.Item1.Image is not null)
                (await _fileService.DeleteFile(tuple.Item1.Image.Path))
                    .OnFailure(() =>
                    {
                        //! Handle Error On Delete File 
                        _logger.LogError("Can't delete the old Popup image with the path : {path}", tuple.Item1.Image.Path);
                    });
            return Result.Ok(tuple);
        })
        .OnSuccessAsync(async (tuple) =>
        {
            PopUpAsset popupAsset = tuple.Item1;
            FileData fileData = tuple.Item2;
            popupAsset.Image = fileData;
            return (await _appAssetsRepo.UpdatePopupAssetData(popupAsset)).MapTo(popupAsset);
        });
    }
}
