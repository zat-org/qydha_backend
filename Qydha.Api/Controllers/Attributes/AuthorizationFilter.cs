namespace Qydha.API.Controllers.Attributes;

public class AuthorizationFilter(IUserRepo userRepo, IAdminUserRepo adminUserRepo) : IAsyncAuthorizationFilter
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IAdminUserRepo _adminUserRepo = adminUserRepo;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext ctx)
    {
        IEnumerable<AuthorizationAttribute> authAttributes = ctx.ActionDescriptor.EndpointMetadata.OfType<AuthorizationAttribute>();

        if (authAttributes is null || !authAttributes.Any()) return;

        #region AuthN
        if (ctx.HttpContext.User.Identity is null || !ctx.HttpContext.User.Identity.IsAuthenticated)
        {
            ctx.Result = new UnauthorizedObjectResult(new Error()
            {
                Code = ErrorType.InvalidAuthToken,
                Message = "Invalid Token"
            });
            return;
        }

        var userIdStr = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null || !Guid.TryParse(userIdStr, out Guid userId))
        {
            ctx.Result = new UnauthorizedObjectResult(new Error()
            {
                Code = ErrorType.InvalidAuthToken,
                Message = "Invalid Token User ID"
            });
            return;
        }
        #endregion


        #region AuthZ

        AuthZUserType _role = authAttributes.Aggregate(AuthZUserType.User, (acc, attr) => attr.Role);

        switch (_role)
        {
            case AuthZUserType.User:
                Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
                if (getUserRes.IsFailure)
                {
                    ctx.Result = new UnauthorizedObjectResult(getUserRes.Error);
                    return;
                }
                ctx.HttpContext.Items["User"] = getUserRes.Value;
                break;
            case AuthZUserType.Admin:
                Result<AdminUser> getAdminUserRes = await _adminUserRepo.GetByIdAsync(userId);
                if (getAdminUserRes.IsFailure)
                {
                    ctx.Result = new UnauthorizedObjectResult(getAdminUserRes.Error);
                    return;
                }
                ctx.HttpContext.Items["User"] = getAdminUserRes.Value;
                break;
        }
        #endregion

    }
}