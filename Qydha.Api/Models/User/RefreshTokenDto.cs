namespace Qydha.API.Models;
public class RefreshTokenDto
{
    public string? RefreshToken { get; set; }
    public string JwtToken { get; set; } = null!;
}