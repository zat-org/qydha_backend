
namespace Qydha.Domain.Services.Implementation;
public class ServiceAccountsService(IServiceAccountRepo serviceAccountRepo, TokenManager tokenManager) : IServiceAccountsService
{

    private readonly IServiceAccountRepo _serviceAccountRepo = serviceAccountRepo;
    private readonly TokenManager _tokenManager = tokenManager;

    public Task<Result> CheckServiceHasPermission(Guid accountId, ServiceAccountPermission permission) =>
        _serviceAccountRepo.CheckServiceHasPermission(accountId, permission);

    public async Task<Result<(ServiceAccount serviceAccount, string jwtToken)>> CreateServiceAccount(string name, string description, List<ServiceAccountPermission> permissions) =>
        (await _serviceAccountRepo.SaveAsync(new ServiceAccount(name, description, permissions)))
            .OnSuccess((sa) => Result.Ok((sa, _tokenManager.Generate(sa))));

    public async Task<Result<PagedList<ServiceAccount>>> GetServiceAccountsPage(PaginationParameters parameters) =>
        await _serviceAccountRepo.GetServiceAccounts(parameters);
    public async Task<Result<string>> GetNewTokenById(Guid id) =>
        (await _serviceAccountRepo.GetById(id))
            .OnSuccess(sa =>
            {
                string token = _tokenManager.Generate(sa);
                return Result.Ok(token);
            });

    public async Task<Result> DeleteById(Guid id) => await _serviceAccountRepo.DeleteById(id);
    public async Task<Result<ServiceAccount>> UpdateAsync(Guid id, string name, string description, List<ServiceAccountPermission> permissions)
    {
        return (await _serviceAccountRepo.GetById(id))
        .OnSuccessAsync(async sa =>
        {
            sa.Permissions = permissions;
            sa.Name = name;
            sa.Description = description;
            return await _serviceAccountRepo.UpdateAsync(sa);
        });
    }

}