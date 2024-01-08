namespace Qydha.Domain.Settings;

public class JWTSettings
{
    public string SecretForKey { get; set; } = null!;
    public string Issuer { get; set; } = "https://localhost:7002";
    public string Audience { get; set; } = "qydhaApi";
    public int SecondsForValidityOfToken { get; set; } = 10 * 24 * 60 * 60;

}