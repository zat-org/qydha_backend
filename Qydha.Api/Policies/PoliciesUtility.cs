namespace Qydha.API.Policies;
public static class PoliciesUtility
{
    public static bool IsServiceAccountHasPermission(AuthorizationHandlerContext context, IServiceAccountsService _serviceAccountService)
    {
        if (context.User.HasClaim(c => c.Type == ClaimsNamesConstants.TokenType && c.Value == ServiceAccount.TokenType)
            && context.Resource is HttpContext httpContext)
        {
            PermissionAttribute permissionAttr = httpContext.GetEndpoint()?.Metadata.GetMetadata<PermissionAttribute>() ??
                        throw new InvalidOperationException("Can't Find PermissionAttribute In MetaData");

            var checkServiceAccountRes = context.User
                .GetUserIdentifier()
                .OnSuccessAsync(async (accountId) =>
                    await _serviceAccountService.CheckServiceHasPermission(accountId, permissionAttr.Permission));

            return checkServiceAccountRes.IsSuccess;
        }
        return false;
    }
    public static bool IsUser(AuthorizationHandlerContext context) =>
        context.User.IsUserToken() && context.User.HasUserRole();

    public static bool IsAdmin(AuthorizationHandlerContext context) =>
        context.User.IsUserToken() && context.User.HasAdminRole();

    public static bool IsSubscribedUser(AuthorizationHandlerContext context, IUserService _userService)
    {
        if (!context.User.IsUserToken()) return false;

        var checkIsUserSubscribedRes = context.User
            .GetUserIdentifier()
            .OnSuccessAsync(_userService.IsUserSubscribed);

        return checkIsUserSubscribedRes.IsSuccess;
    }
}