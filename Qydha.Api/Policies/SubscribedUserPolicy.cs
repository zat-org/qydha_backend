namespace Qydha.API.Policies;
public class SubscribedUserPolicyHandler(IUserService userService) : AuthorizationHandler<SubscribedUserPolicyRequirement>
{

    private readonly IUserService _userService = userService;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscribedUserPolicyRequirement requirement)
    {
        if (PoliciesUtility.IsSubscribedUser(context, _userService))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

public class SubscribedUserPolicyRequirement : IAuthorizationRequirement { }