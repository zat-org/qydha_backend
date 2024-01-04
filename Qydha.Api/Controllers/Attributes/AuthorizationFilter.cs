using System.Security.Claims;

namespace Qydha.API.Controllers.Attributes;

public class AuthorizationFilter(IUserRepo userRepo, IAdminUserRepo adminUserRepo) : IAsyncAuthorizationFilter
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IAdminUserRepo _adminUserRepo = adminUserRepo;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext ctx)
    {
        IEnumerable<AuthAttribute> authAttributes = ctx.ActionDescriptor.EndpointMetadata.OfType<AuthAttribute>();
        IEnumerable<AllowAnonymousAttribute> allowAnonymousAttributes = ctx.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>();

        if (!authAttributes.Any() || allowAnonymousAttributes.Any()) return;

        AuthAttribute attr = authAttributes.LastOrDefault()!;


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
        List<string> userStringRoles = ctx.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        string? isAnonymousUserStr = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "isAnonymous")?.Value;
        if (isAnonymousUserStr is not null && bool.TryParse(isAnonymousUserStr, out bool isAnonymousUser))
            userStringRoles.Add(!isAnonymousUser ? "RegularUser" : "AnonymousUser");


        int userRole = userStringRoles.Select(stringRole =>
            {
                if (Enum.TryParse(stringRole, out SystemUserRoles userRole))
                    return (int)userRole;
                else
                    return 0;
            })
            .Aggregate((acc, role) => acc | role);

        if (userStringRoles.Count == 0 || userRole == 0)
        {
            ctx.Result = new UnauthorizedObjectResult(new Error()
            {
                Code = ErrorType.InvalidAuthToken,
                Message = "Invalid Token Role"
            });
            return;
        }


        if ((userRole & (int)SystemUserRoles.User) != 0)
        {
            Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
            if (getUserRes.IsFailure)
            {
                ctx.Result = new UnauthorizedObjectResult(new Error()
                {
                    Code = ErrorType.InvalidAuthToken,
                    Message = getUserRes.Error.Message
                });
                return;
            }
            ctx.HttpContext.Items["User"] = getUserRes.Value;
        }
        else if ((userRole & (int)SystemUserRoles.Admin) != 0)
        {
            Result<AdminUser> getAdminUserRes = await _adminUserRepo.GetByIdAsync(userId);
            if (getAdminUserRes.IsFailure)
            {
                ctx.Result = new UnauthorizedObjectResult(new Error()
                {
                    Code = ErrorType.InvalidAuthToken,
                    Message = getAdminUserRes.Error.Message
                });
                return;
            }
            ctx.HttpContext.Items["User"] = getAdminUserRes.Value;
        }



        if (attr.Role == SystemUserRoles.All) return;

        if (((int)attr.Role & userRole) == 0)
        {
            ctx.Result = new ObjectResult(new Error()
            {
                Code = ErrorType.InvalidActionOrForbidden,
                Message = "the targeted action is not valid for provided user token."
            })
            {
                StatusCode = 403
            };
            return;
        }
        #endregion
    }
}