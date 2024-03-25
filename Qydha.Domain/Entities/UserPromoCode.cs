namespace Qydha.Domain.Entities;
public class UserPromoCode
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public int NumberOfDays { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpireAt { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public UserPromoCode()
    {

    }
    public UserPromoCode(Guid userId, string code, int numberOfDays, DateTimeOffset expireAt)
    {
        UserId = userId;
        Code = code;
        NumberOfDays = numberOfDays;
        ExpireAt = expireAt;
        CreatedAt = DateTimeOffset.UtcNow;
        UsedAt = null;
    }
}
