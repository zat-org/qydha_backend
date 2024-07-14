namespace Qydha.API.Controllers;

[ApiController]
[Route("service-account/")]
[Authorize(Roles = RoleConstants.SuperAdmin)]
public class ServiceAccountController(IServiceAccountsService serviceAccountsService) : ControllerBase
{
    private readonly IServiceAccountsService _serviceAccountsService = serviceAccountsService;

    [HttpGet("permissions/")]
    public IActionResult GetServiceAccountPermissions()
    {
        return Ok(new
        {
            data = new
            {
                permissions = Enum
                .GetValues(typeof(ServiceAccountPermission))
                .Cast<ServiceAccountPermission>()
                .Select(c => c.ToString())
                .ToList()
            },
            message = "permissions fetched successfully."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServiceAccounts([FromQuery] PaginationParameters parameters)
    {
        return (await _serviceAccountsService.GetServiceAccountsPage(parameters))
        .Resolve(
            (accounts) =>
            {
                return Ok(new
                {
                    data = new
                    {
                        accounts = new ServiceAccountPage(accounts, accounts.TotalCount, accounts.CurrentPage, accounts.PageSize)
                    },
                    message = "Service Accounts fetched successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceAccount([FromBody] ServiceAccountDto dto)
    {
        return (await _serviceAccountsService.CreateServiceAccount(dto.Name, dto.Description, dto.Permissions))
        .Resolve(
            (tuple) =>
            {
                return Ok(new
                {
                    data = new { ServiceAccount = tuple.serviceAccount, Token = tuple.jwtToken },
                    message = "Service Account Created successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateServiceAccount([FromRoute] Guid id, [FromBody] ServiceAccountDto dto)
    {
        return (await _serviceAccountsService.UpdateAsync(id, dto.Name, dto.Description, dto.Permissions))
        .Resolve(
            (account) =>
            {
                return Ok(new
                {
                    data = new { ServiceAccount = account },
                    message = "Service Account Updated successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }
    [HttpGet("{id}/token")]
    public async Task<IActionResult> GetNewTokenForServiceAccount([FromRoute] Guid id)
    {
        return (await _serviceAccountsService.GetNewTokenById(id))
        .Resolve(
            (token) =>
            {
                return Ok(new
                {
                    data = new { token },
                    message = "Token Created successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceAccount([FromRoute] Guid id)
    {
        return (await _serviceAccountsService.DeleteById(id))
        .Resolve(
            () =>
            {
                return Ok(new
                {
                    data = new { },
                    message = "Service Account Deleted successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }

}
