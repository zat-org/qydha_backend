using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;

namespace Qydha.API.Attributes;

public class AuthorizationFilter(IUserRepo userRepo, IAdminUserRepo adminUserRepo, ILogger<AuthorizationFilter> logger, IOptions<JWTSettings> options) : IAuthorizationFilter
{
    private readonly ILogger<AuthorizationFilter> _logger = logger;
    private readonly JWTSettings _jwtSettings = options.Value;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IAdminUserRepo _adminUserRepo = adminUserRepo;

    public void OnAuthorization(AuthorizationFilterContext ctx)
    {
        IEnumerable<AuthAttribute> authAttributes = ctx.ActionDescriptor.EndpointMetadata.OfType<AuthAttribute>();
        IEnumerable<AllowAnonymousAttribute> allowAnonymousAttributes = ctx.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>();

        if (!authAttributes.Any() || allowAnonymousAttributes.Any()) return;

        GetJwtTokenFromHeaders(ctx)
        .OnSuccess(ValidateToken)
        .OnSuccess((principal) =>
        {
            var userIdStr = principal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdStr is null || !Guid.TryParse(userIdStr, out Guid userId))
            {
                _logger.LogInformation("401 : No userId value");
                return Result.Fail(new InvalidAuthTokenError());
            }
            return Result.Ok((principal, userId)); 
        })
        .OnSuccess((tuple) =>
        {
            List<string> userStringRoles = tuple.principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            string? isAnonymousUserStr = tuple.principal.Claims.FirstOrDefault(c => c.Type == "isAnonymous")?.Value;

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
                _logger.LogInformation("401 : No Role value");
                return Result.Fail(new InvalidAuthTokenError());
            }
            return Result.Ok((tuple.principal, tuple.userId, userRole));
        })
        .OnSuccessAsync(async (tuple) =>
        {
            var principal = tuple.principal;
            var userId = tuple.userId;
            var userRole = tuple.userRole;

            if ((userRole & (int)SystemUserRoles.User) != 0)
            {
                Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
                if (getUserRes.IsFailed)
                {
                    _logger.LogInformation("401 : User Not found with id : {id} with failure message: {msg}", userId, getUserRes.Errors[0].Message);
                    return Result.Fail(new InvalidAuthTokenError());
                }
                ctx.HttpContext.Items["User"] = getUserRes.Value;
                return Result.Ok(tuple);
            }
            else if ((userRole & (int)SystemUserRoles.Admin) != 0)
            {
                Result<AdminUser> getAdminUserRes = await _adminUserRepo.GetByIdAsync(userId);
                if (getAdminUserRes.IsFailed)
                {
                    _logger.LogInformation("401 : Admin Not found with id : {id} with failure message: {msg}", userId, getAdminUserRes.Errors[0].Message);
                    return Result.Fail(new InvalidAuthTokenError());
                }
                ctx.HttpContext.Items["User"] = getAdminUserRes.Value;
                return Result.Ok(tuple);
            }
            return Result.Fail(new InvalidAuthTokenError());
        })
        .OnSuccess((tuple) =>
        {
            AuthAttribute attr = authAttributes.LastOrDefault()!;
            if (attr.Role == SystemUserRoles.All) return Result.Ok(tuple);

            if (((int)attr.Role & tuple.userRole) == 0)
            {
                _logger.LogInformation("403 user forbidden from this action userId : {id} ", tuple.userId);
                return Result.Fail(new ForbiddenError());
            }
            else
                return Result.Ok(tuple);
        })
        .Handle(
            (tuple) => ctx.HttpContext.User = tuple.principal,
            (errors) => ctx.Result = errors.First().Handle(ctx.HttpContext.TraceIdentifier)
        );

    }
    private Result<string> GetJwtTokenFromHeaders(AuthorizationFilterContext ctx)
    {
        string token = "";
        if (ctx.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeaders))
        {
            string? bearerToken = authorizationHeaders.FirstOrDefault(header => header is not null && header.StartsWith("Bearer "));
            if (!string.IsNullOrEmpty(bearerToken))
            {
                token = bearerToken["Bearer ".Length..];
                return Result.Ok(token);
            }
        }
        _logger.LogInformation("401 : No Token provided");
        return Result.Fail(new InvalidAuthTokenError());
    }

    private Result<ClaimsPrincipal> ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretForKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return Result.Ok(principal);
        }
        catch (Exception exp)
        {
            _logger.LogInformation("401 : Invalid token Signature with Token : {token} , with Message : {msg}", token, exp.Message);
            return Result.Fail<ClaimsPrincipal>(new InvalidAuthTokenError());
        }
    }

}