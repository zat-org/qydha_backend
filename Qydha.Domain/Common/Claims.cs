namespace Qydha.Domain.Common;

public interface IClaimable
{
    IEnumerable<Claim> GetClaims();
}
public static class ClaimsNamesConstants
{
    public const string TokenType = "TokenType";
}
