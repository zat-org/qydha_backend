namespace Qydha.Domain.Repositories;
public interface IServiceAccountRepo
{
    Task<Result> CheckServiceHasPermission(Guid accountId, ServiceAccountPermission permission);
    Task<Result<ServiceAccount>> SaveAsync(ServiceAccount account);
    Task<Result<PagedList<ServiceAccount>>> GetServiceAccounts(PaginationParameters parameters);
    Task<Result<ServiceAccount>> GetById(Guid id);
    Task<Result> DeleteById(Guid id);
    Task<Result<ServiceAccount>> UpdateAsync(ServiceAccount account);
}
