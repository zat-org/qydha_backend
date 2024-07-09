namespace Qydha.Domain.Services.Contracts;
public interface IServiceAccountsService
{
    Task<Result> CheckServiceHasPermission(Guid accountId, ServiceAccountPermission permission);
    Task<Result<(ServiceAccount serviceAccount, string jwtToken)>> CreateServiceAccount(string name, string description, List<ServiceAccountPermission> permissions);
    Task<Result<PagedList<ServiceAccount>>> GetServiceAccountsPage(PaginationParameters parameters);
    Task<Result<string>> GetNewTokenById(Guid id);
    Task<Result> DeleteById(Guid id);
    Task<Result<ServiceAccount>> UpdateAsync(Guid id, string name, string description, List<ServiceAccountPermission> permissions);

}