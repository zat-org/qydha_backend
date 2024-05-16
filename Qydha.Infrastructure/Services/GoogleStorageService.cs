using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
namespace Qydha.Infrastructure.Services;
public class GoogleStorageService
{
    private readonly StorageClient _storageClient;

    public GoogleStorageService(string jsonKeyFilePath)
    {
        GoogleCredential credential = GoogleCredential.FromFile(jsonKeyFilePath);
        _storageClient = StorageClient.Create(credential);
    }

    public StorageClient GetStorageClient() => _storageClient;

}