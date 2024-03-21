namespace Qydha.Domain.Entities;
public class UserPromoCode
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public int NumberOfDays { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    public Guid UserId { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public UserPromoCode()
    {

    }
    public UserPromoCode(Guid userId, string code, int numberOfDays, DateTime expireAt)
    {
        UserId = userId;
        Code = code;
        NumberOfDays = numberOfDays;
        ExpireAt = expireAt;
        CreatedAt = DateTime.UtcNow;
        UsedAt = null;
    }
}
