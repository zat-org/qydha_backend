using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Qydha.Domain.Services.Implementation;

public class TokenManager
{
    private readonly JWTSettings _jwtSettings;
    public TokenManager(IOptions<JWTSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public string Generate(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_jwtSettings.SecretForKey)
        );

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddSeconds(_jwtSettings.SecondsForValidityOfToken),
            signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

}
