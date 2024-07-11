
namespace Qydha.API.Extensions;

public static class ClaimPrincipalExtensions
{
    public static Result<Guid> GetUserIdentifier(this ClaimsPrincipal user)
    {
        var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (nameIdentifier == null) return Result.Fail(new InvalidAuthTokenError());
        else return Result.Ok(Guid.Parse(nameIdentifier));
    }
    public static bool HasRoles(this ClaimsPrincipal user, List<UserRoles> targetRoles)
    {
        if (targetRoles.Count == 0) return false;
        var stringRoles = targetRoles.Select(x => x.ToString().ToLower()).ToArray();
        return user.Claims.Any(c => c.Type == ClaimTypes.Role && stringRoles.Contains(c.Value.ToLower()));
    }
    public static bool HasRoles(this ClaimsPrincipal user, UserRoles targetRole) =>
        user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.Equals(targetRole.ToString(), StringComparison.OrdinalIgnoreCase));
    public static bool IsUserToken(this ClaimsPrincipal user) =>
        user.HasClaim(c => c.Type == ClaimsNamesConstants.TokenType && c.Value == User.TokenType);
    public static bool IsServiceAccountToken(this ClaimsPrincipal user) =>
        user.HasClaim(c => c.Type == ClaimsNamesConstants.TokenType && c.Value == ServiceAccount.TokenType);
    public static bool HasAdminRole(this ClaimsPrincipal user) =>
        user.HasRoles([UserRoles.StaffAdmin, UserRoles.SuperAdmin]);
    public static bool HasUserRole(this ClaimsPrincipal user) => user.HasRoles(UserRoles.User);

}