namespace Qydha.Domain.Entities;

public class InfluencerCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string NormalizedCode { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExpireAt { get; set; }
    public int NumberOfDays { get; set; }
    public int MaxInfluencedUsersCount { get; set; }
    public int? CategoryId { get; set; }
    public InfluencerCodeCategory? Category { get; set; }
    public InfluencerCode() { }
    public InfluencerCode(string code, int numOfDays, DateTimeOffset? expireDate, int maxInfluencedUsers, int? categoryId)
    {
        Code = code;
        NormalizedCode = code.ToUpper();
        CreatedAt = DateTimeOffset.UtcNow;
        ExpireAt = expireDate;
        NumberOfDays = numOfDays;
        MaxInfluencedUsersCount = maxInfluencedUsers;
        CategoryId = categoryId;
    }
}