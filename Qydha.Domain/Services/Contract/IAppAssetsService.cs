namespace Qydha.Domain.Services.Contracts;

public interface IAppAssetsService
{
    Task<Result<BookAsset>> GetBalootBookData();
    Task<Result<BookAsset>> UpdateBalootBookData(IFormFile bookFile);
}
