
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

    public async Task<Result<int>> GetInfluencerCodeUsageByUserIdCountAsync(Guid userId, string code)
    {
        return Result.Ok(await _dbCtx.Purchases
           .Where(purchase => purchase.UserId == userId && purchase.Type == "Influencer" && purchase.ProductSku == code)
           .CountAsync());
    }

    public async Task<Result<int>> GetInfluencerCodeUsageByAllUsersCountAsync(string code)
    {
        return Result.Ok(await _dbCtx.Purchases
           .Where(purchase => purchase.Type == "Influencer" && purchase.ProductSku == code)
           .CountAsync());
    }

    public async Task<Result<int>> GetInfluencerCodeUsageCountByCategoryForUserAsync(Guid userId, int categoryId)
    {
        //! TODO Refactor here by create relation between purchases and influencer Codes
        List<string> purchases = await _dbCtx.Purchases
           .Where(purchase => purchase.UserId == userId && purchase.Type == "Influencer")
           .Select(p => p.ProductSku)
           .ToListAsync();
        List<string> codes = await _dbCtx.InfluencerCodes
           .Where(code => code.CategoryId == categoryId)
           .Select(code => code.Code)
           .ToListAsync();
        int counter = 0;
        purchases.ForEach((p) =>
        {
            if (codes.Contains(p))
                counter++;
        });
        return Result.Ok(counter);
    } 

    public async Task<Result<Purchase>> AddAsync(Purchase purchase)
    {
        await _dbCtx.Purchases.AddAsync(purchase);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(purchase);
    }
}
