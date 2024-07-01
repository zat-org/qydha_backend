
namespace Qydha.API.Extensions;

public static class ClaimPrincipalExtensions
{
    public static Result<Guid> GetUserIdentifier(this ClaimsPrincipal user)
    {
        var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (nameIdentifier == null) return Result.Fail(new InvalidAuthTokenError());
        else return Result.Ok(Guid.Parse(nameIdentifier));
    }

}