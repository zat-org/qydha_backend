namespace Qydha.API.Policies;
public class UserOrPermissionPolicyHandler(IServiceAccountsService serviceAccountService) : AuthorizationHandler<UserOrPermissionPolicyRequirement>
{
    private readonly IServiceAccountsService _serviceAccountService = serviceAccountService;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOrPermissionPolicyRequirement requirement)
    {
        if (PoliciesUtility.IsUser(context))
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

public class UserOrPermissionPolicyRequirement : IAuthorizationRequirement { }
