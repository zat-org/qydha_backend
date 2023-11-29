namespace Qydha.Domain.Repositories;

public interface IPurchaseRepo
{
    Task<Result<Purchase>> AddAsync(Purchase purchase);
}
