namespace Qydha.Domain.Entities;

public class InfluencerCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string NormalizedCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public int NumberOfDays { get; set; }
    public int MaxInfluencedUsersCount { get; set; }
    public int? CategoryId { get; set; }
    public InfluencerCodeCategory? Category { get; set; }
    public InfluencerCode() { }
    public InfluencerCode(string code, int numOfDays, DateTime? expireDate, int maxInfluencedUsers, int? categoryId)
    {
        Code = code;
        NormalizedCode = code.ToUpper();
        CreatedAt = DateTime.UtcNow;
        ExpireAt = expireDate;
        NumberOfDays = numOfDays;
        MaxInfluencedUsersCount = maxInfluencedUsers;
        CategoryId = categoryId;
    }
}