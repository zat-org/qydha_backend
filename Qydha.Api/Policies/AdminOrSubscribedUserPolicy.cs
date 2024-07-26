namespace Qydha.API.Policies;
public class AdminOrSubscribedUserPolicyHandler(IUserService userService) : AuthorizationHandler<AdminOrSubscribedUserPolicyRequirement>
{

    private readonly IUserService _userService = userService;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrSubscribedUserPolicyRequirement requirement)
    {
        if (PoliciesUtility.IsAdmin(context))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        if (PoliciesUtility.IsSubscribedUser(context, _userService))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
}

public class AdminOrSubscribedUserPolicyRequirement : IAuthorizationRequirement { }