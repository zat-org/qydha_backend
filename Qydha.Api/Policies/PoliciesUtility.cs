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
    public static bool IsAdmin(AuthorizationHandlerContext context) =>
        context.User.HasClaim(c => c.Type == ClaimsNamesConstants.TokenType && c.Value == User.TokenType) &&
        (context.User.IsInRole(UserRoles.StaffAdmin.ToString()) || context.User.IsInRole(UserRoles.SuperAdmin.ToString()));
    public static bool IsUserWithAnyRole(AuthorizationHandlerContext context) => 
        context.User.HasClaim(c => c.Type == ClaimsNamesConstants.TokenType && c.Value == User.TokenType);
    public static bool IsSubscribedUser(AuthorizationHandlerContext context, IUserService _userService)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimsNamesConstants.TokenType && c.Value == User.TokenType))
            return false;

        var checkIsUserSubscribedRes = context.User
            .GetUserIdentifier()
            .OnSuccessAsync(_userService.IsUserSubscribed);

        return checkIsUserSubscribedRes.IsSuccess;
    }
}