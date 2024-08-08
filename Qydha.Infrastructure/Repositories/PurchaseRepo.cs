
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
    public async Task<Result<Purchase>> GetByPurchaseIdAsync(string purchaseId)
    {
        var purchase = await _dbCtx.Purchases.SingleOrDefaultAsync(purchase => purchase.IAPHubPurchaseId == purchaseId);
        if (purchase == null) return Result.Fail(new EntityNotFoundError<string>(purchaseId, nameof(Purchase)));
        return Result.Ok(purchase);
    }
    public async Task<Result<Purchase>> AddAsync(Purchase purchase)
    {
        if (!_dbCtx.Users.Any(u => u.Id == purchase.UserId))
            return Result.Fail(new EntityNotFoundError<Guid>(purchase.UserId, nameof(User)));
        await _dbCtx.Purchases.AddAsync(purchase);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(purchase);
    }

    public async Task<Result> RefundByPurchaseIdAsync(string purchaseId, DateTimeOffset refundedAt)
    {
        var affected = await _dbCtx.Purchases.Where(purchase => purchase.IAPHubPurchaseId == purchaseId).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(p => p.RefundedAt, refundedAt)
        );
        if (affected == 0) return Result.Fail(new EntityNotFoundError<string>(purchaseId, nameof(Purchase)));
        return Result.Ok();
    }
}
