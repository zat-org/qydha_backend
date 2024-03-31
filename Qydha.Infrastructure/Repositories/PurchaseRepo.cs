
namespace Qydha.Infrastructure.Repositories;
public class PurchaseRepo(QydhaContext qydhaContext, ILogger<PurchaseRepo> logger) : IPurchaseRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<PurchaseRepo> _logger = logger;
    public async Task<Result<IEnumerable<Purchase>>> GetAllByUserIdAsync(Guid userId)
    {
        var codes = await _dbCtx.Purchases
           .Where(purchase => purchase.UserId == userId).OrderByDescending(purchase => purchase.PurchaseDate).ToListAsync();
        return Result.Ok((IEnumerable<Purchase>)codes);
    }
    public async Task<Result<Purchase>> AddAsync(Purchase purchase)
    {
        await _dbCtx.Purchases.AddAsync(purchase);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(purchase);
    }
}
