namespace Qydha.API.Extensions;

public static class CookiesExtensions
{
    public static readonly string RefreshTokenCookieName = "refreshToken";

    public static void AddRefreshToken(this IResponseCookies cookies, string token, DateTimeOffset expireAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expireAt
        };
        cookies.Append(RefreshTokenCookieName, token, cookieOptions);
    }
}
