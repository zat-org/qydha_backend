using Google.Cloud.Storage.V1;
namespace Qydha.Infrastructure.Services;

public class GoogleCloudFileService(GoogleStorageService googleStorageService, IOptions<GoogleStorageSettings> options, ILogger<GoogleCloudFileService> logger) : IFileService
{

    private readonly StorageClient _client = googleStorageService.GetStorageClient();
    private readonly ILogger<GoogleCloudFileService> _logger = logger;
    private readonly GoogleStorageSettings _googleStorageSettings = options.Value;

    public async Task<Result> DeleteFile(string path)
    {
        try
        {
            await _client.DeleteObjectAsync(_googleStorageSettings.BucketName, path);
            return Result.Ok();
        }
        catch (Exception exp)
        {
            _logger.LogCritical("Error in Deleting file in bucket : {bucketName} with path : {filePath} with exception message : {expMsg} ", _googleStorageSettings.BucketName, path, exp.Message);
            return Result.Fail(new FileStorageOperationError(FileStorageAction.Delete, path).CausedBy(exp));
        }
    }

    public async Task<Result<FileData>> UploadFile(string folderPath, IFormFile file)
    {
        try
        {
            using var ms = new MemoryStream();
            string fileName = $"{Path.GetRandomFileName()}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            string pathToFileInTheBucket = $"{folderPath}/{fileName}";
            await file.CopyToAsync(ms);
            UploadObjectOptions options = new() { PredefinedAcl = PredefinedObjectAcl.PublicRead };
            var res = await _client.UploadObjectAsync(_googleStorageSettings.BucketName, pathToFileInTheBucket, file.ContentType, ms, options);
            return Result.Ok(new FileData() { Url = res.MediaLink, Path = pathToFileInTheBucket });
        }
        catch (Exception exp)
        {
            _logger.LogCritical("Error in Uploading file in bucket : {bucketName} with Folder path : {folderPath} with exception message : {expMsg} ", _googleStorageSettings.BucketName, folderPath, exp.Message);
            return Result.Fail(new FileStorageOperationError(FileStorageAction.Upload, folderPath).CausedBy(exp));
        }
    }
}
