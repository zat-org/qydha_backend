namespace Qydha.Domain.Settings;

public class JWTSettings
{
    public string SecretForKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpireAfterMinutes { get; set; }
    public int RefreshTokenExpireAfterDays { get; set; }
    public int RefreshTokenArraySize { get; set; }


}