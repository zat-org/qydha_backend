namespace Qydha.Domain.Entities;

public class UserPromoCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Number_Of_Days { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime Expire_At { get; set; }
    public Guid User_Id { get; set; }
    public DateTime? Used_At { get; set; }

    public UserPromoCode()
    {

    }
    public UserPromoCode(Guid userId, string code, int numberOfDays, DateTime expireAt)
    {
        User_Id = userId;
        Code = code;
        Number_Of_Days = numberOfDays;
        Expire_At = expireAt;
        Created_At = DateTime.UtcNow;
        Used_At = null;
    }
}
