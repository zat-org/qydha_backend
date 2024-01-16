namespace Qydha.Domain.Entities;
[Table("InfluencerCodes")]
[NotFoundError(ErrorType.InfluencerCodeNotFound)]

public class InfluencerCode : DbEntity<InfluencerCode>
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = null!;

    [Column("normalized_code")]
    public string NormalizedCode { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("expire_at")]
    public DateTime? ExpireAt { get; set; }

    [Column("number_of_days")]
    public int NumberOfDays { get; set; }

    [Column("max_influenced_users_count")]
    public int MaxInfluencedUsersCount { get; set; }


    public InfluencerCode()
    {

    }
    public InfluencerCode(string code, int numOfDays, DateTime? expireDate, int maxInfluencedUsers)
    {
        Code = code;
        NormalizedCode = code.ToUpper();
        CreatedAt = DateTime.UtcNow;
        ExpireAt = expireDate;
        NumberOfDays = numOfDays;
        MaxInfluencedUsersCount = maxInfluencedUsers;
    }
}