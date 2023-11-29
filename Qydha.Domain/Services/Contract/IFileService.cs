
namespace Qydha.Domain.Services.Contracts;

public interface IFileService
{
    public Task<Result<FileData>> UploadFile(string uploadPath, IFormFile file);
    public Task<Result> DeleteFile(string path);

}
