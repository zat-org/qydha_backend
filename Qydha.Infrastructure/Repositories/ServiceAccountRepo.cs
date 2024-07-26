
namespace Qydha.Infrastructure.Repositories;
public class ServiceAccountRepo(QydhaContext qydhaContext, ILogger<ServiceAccountRepo> logger) : IServiceAccountRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<ServiceAccountRepo> _logger = logger;
    public async Task<Result> CheckServiceHasPermission(Guid accountId, ServiceAccountPermission permission)
    {
        bool hasPermission = await _dbCtx.ServiceAccounts.AnyAsync(g => g.Id == accountId && g.Permissions.Contains(permission));
        return hasPermission ? Result.Ok() : Result.Fail(new ForbiddenError());
    }

    public async Task<Result<ServiceAccount>> SaveAsync(ServiceAccount account)
    {
        _dbCtx.ServiceAccounts.Add(account);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(account);
    }
    public async Task<Result<PagedList<ServiceAccount>>> GetServiceAccounts(PaginationParameters parameters)
    {
        PagedList<ServiceAccount> accounts =
            await _dbCtx.GetPagedData(_dbCtx.ServiceAccounts, parameters.PageNumber, parameters.PageSize);
        return Result.Ok(accounts);
    }
    public async Task<Result<ServiceAccount>> GetById(Guid id)
    {
        ServiceAccount? account = await _dbCtx.ServiceAccounts.SingleOrDefaultAsync(sa => sa.Id == id);
        return account is null ?
            Result.Fail(new EntityNotFoundError<Guid>(id, nameof(ServiceAccount))) :
            Result.Ok(account);
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var affected = await _dbCtx.ServiceAccounts.Where(sa => sa.Id == id).ExecuteDeleteAsync();
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(id, nameof(ServiceAccount)));
    }
    public async Task<Result<ServiceAccount>> UpdateAsync(ServiceAccount account)
    {
        _dbCtx.Update(account);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(account);
    }
}