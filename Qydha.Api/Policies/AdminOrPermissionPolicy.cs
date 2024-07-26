namespace Qydha.API.Policies;
public class AdminOrPermissionPolicyHandler(IServiceAccountsService serviceAccountService) : AuthorizationHandler<AdminOrPermissionPolicyRequirement>
{
    private readonly IServiceAccountsService _serviceAccountService = serviceAccountService;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrPermissionPolicyRequirement requirement)
    {
        if (PoliciesUtility.IsAdmin(context))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        if (PoliciesUtility.IsServiceAccountHasPermission(context, _serviceAccountService))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
}

public class AdminOrPermissionPolicyRequirement : IAuthorizationRequirement { }
