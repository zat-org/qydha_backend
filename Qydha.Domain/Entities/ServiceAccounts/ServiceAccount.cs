namespace Qydha.Domain.Entities;

public class ServiceAccount : IClaimable
{
    public ServiceAccount() { }
    public ServiceAccount(string name, string description, List<ServiceAccountPermission> permissions)
    {
        Name = name;
        Description = description;
        Permissions = permissions;
    }

    public const string TokenType = "ServiceAccountToken";
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<ServiceAccountPermission> Permissions { get; set; } = [];

    public IEnumerable<Claim> GetClaims() => [
        new(ClaimTypes.NameIdentifier, Id.ToString()),
        new(ClaimTypes.Name, Name),
        new(ClaimsNamesConstants.TokenType, TokenType)
    ];

}
public enum ServiceAccountPermission
{
    AnonymousBalootGameCRUDs,
    ReadPopup,
    ReadBalootGameStatistics,
    ReadBalootGameTimeline,
    ReadPublicNotifications,
    ClickOnPublicNotification,
    CheckUserNameAvailable,
    LoginWithQydha
}