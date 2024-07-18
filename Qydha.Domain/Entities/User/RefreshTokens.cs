namespace Qydha.Domain.Entities;

public class RefreshToken
{
    public RefreshToken(string token, DateTimeOffset expireAt)
    {
        Token = token;
        ExpireAt = expireAt;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    public string Token { get; set; }
    public DateTimeOffset ExpireAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpireAt;
    public bool IsActive => RevokedAt == null && !IsExpired;
}