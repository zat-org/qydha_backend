using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Qydha.Domain.Services.Implementation;

public class TokenManager(IOptions<JWTSettings> jwtSettings)
{
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;

    public string GenerateJwtToken(IClaimable claimable)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretForKey)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claimable.GetClaims()),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireAfterMinutes),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public RefreshToken GenerateRefreshToken()
    {
        var randomNum = new byte[_jwtSettings.RefreshTokenArraySize];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNum);
        string token = Convert.ToBase64String(randomNum);
        return new RefreshToken(token, DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireAfterDays));
    }
    public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretForKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out SecurityToken securityToken);
            var JwtSecurityToken = securityToken as JwtSecurityToken;
            if (JwtSecurityToken == null ||
                !JwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return Result.Fail(new InvalidAuthTokenError());
            return Result.Ok(principal);
        }
        catch (Exception)
        {
            return Result.Fail(new InvalidAuthTokenError());
        }

    }
}
