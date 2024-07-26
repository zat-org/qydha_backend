namespace Qydha.API.Policies;
public class PermissionHandler(IServiceAccountsService serviceAccountService) : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceAccountsService _serviceAccountService = serviceAccountService;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (PoliciesUtility.IsServiceAccountHasPermission(context, _serviceAccountService))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
}

public class PermissionRequirement : IAuthorizationRequirement { }
