namespace Qydha.Infrastructure.Repositories;
public class PurchaseRepo(IDbConnection dbConnection, ILogger<PurchaseRepo> logger) : GenericRepository<Purchase>(dbConnection, logger), IPurchaseRepo
{
    // public async Task<Result<IEnumerable<Purchase>>> GetAllByUserAsync(Guid userId)
    // {
    //     await GetAllAsync()
    // }
}
