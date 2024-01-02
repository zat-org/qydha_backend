namespace Qydha.Domain.Services.Implementation;

public class AppAssetsService(IAppAssetsRepo appAssetsRepo, IFileService fileService, ILogger<AppAssetsService> logger, IOptions<BookSettings> bookOptions) : IAppAssetsService
{
    private readonly IAppAssetsRepo _appAssetsRepo = appAssetsRepo;
    private readonly IFileService _fileService = fileService;
    private readonly ILogger<AppAssetsService> _logger = logger;
    private readonly BookSettings bookOptions = bookOptions.Value;


    public async Task<Result<BookAsset>> GetBalootBookData() => await _appAssetsRepo.GetBalootBookAssetData();
    public async Task<Result<BookAsset>> UpdateBalootBookData(IFormFile bookFile)
    {
        return (await _appAssetsRepo.GetBalootBookAssetData())
        .OnSuccessAsync(async (bookAsset) =>
        {
            if (bookAsset.Path is not null)
                (await _fileService.DeleteFile(bookAsset.Path)).OnFailure(() =>
                {
                    //! Handle Error On Delete File 
                    _logger.LogError("Can't delete the old book with the path : #path", [bookAsset.Path]);
                });
            return await _fileService.UploadFile(bookOptions.FolderPath, bookFile);
        })
        .OnSuccessAsync(async (fileData) =>
        {
            var bookAsset = new BookAsset()
            {
                LastUpdateAt = DateTime.UtcNow,
                Path = fileData.Path,
                Url = fileData.Url
            };
            return (await _appAssetsRepo.UpdateBalootBookAssetData(bookAsset)).MapTo(bookAsset);
        });
    }

}
