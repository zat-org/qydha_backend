namespace Qydha.Domain.Entities;
[Table("user_promo_codes")]
[NotFoundError(ErrorType.UserPromoCodeNotFound)]
public class UserPromoCode
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = null!;

    [Column("number_of_days")]
    public int NumberOfDays { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("expire_at")]
    public DateTime ExpireAt { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("used_at")]
    public DateTime? UsedAt { get; set; }


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
