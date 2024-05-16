namespace Qydha.Domain.Settings;

public class JWTSettings
{
    public string SecretForKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int SecondsForValidityOfToken { get; set; } = 10 * 24 * 60 * 60;

}