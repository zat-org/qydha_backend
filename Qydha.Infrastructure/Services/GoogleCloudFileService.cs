using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace Qydha.Infrastructure.Services;

public class GoogleCloudFileService(GoogleStorageService googleStorageService) : IFileService
{

    private readonly StorageClient _client = googleStorageService.GetStorageClient();
    private readonly string bucketName = "qydha_bucket";

    public async Task<Result> DeleteFile(string path)
    {
        try
        {
            await _client.DeleteObjectAsync(bucketName, path);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new Error()
            {
                Code = ErrorType.FileDeleteError,
                Message = $"Google Cloud File ERROR : Can't Delete File , with message : {e.Message}"
            });
        }
    }

    public async Task<Result<FileData>> UploadFile(string pathInBucket, IFormFile file)
    {
        try
        {
            using var ms = new MemoryStream();

            string fileName = $"({Guid.NewGuid()})-{file.FileName}";
            string pathToFileInTheBucket = $"{pathInBucket}{fileName}";

            await file.CopyToAsync(ms);

            UploadObjectOptions options = new() { PredefinedAcl = PredefinedObjectAcl.PublicRead };

            var res = await _client.UploadObjectAsync(bucketName, pathToFileInTheBucket, file.ContentType, ms, options);

            string publicAccessLink = $"https://storage.googleapis.com/{bucketName}/{pathToFileInTheBucket}";

            return Result.Ok(new FileData() { Url = publicAccessLink, Path = pathToFileInTheBucket });
        }
        catch (Exception e)
        {
            return Result.Fail<FileData>(new()
            {
                Code = ErrorType.FileUploadError,
                Message = $"Google Cloud File ERROR : Can't Upload File , With Message = {e.Message}"
            });
        }
    }
}
