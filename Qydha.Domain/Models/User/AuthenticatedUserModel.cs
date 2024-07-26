namespace Qydha.Domain.Models;
public record AuthenticatedUserModel(User User, string JwtToken, string RefreshToken, DateTimeOffset RefreshTokenExpirationDate);
