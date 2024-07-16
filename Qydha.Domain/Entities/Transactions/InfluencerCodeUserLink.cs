namespace Qydha.Domain.Entities;

public class InfluencerCodeUserLink
{
    public Guid InfluencerCodeId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset UsedAt { get; set; }
    public int NumberOfDays { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
    public virtual InfluencerCode InfluencerCode { get; set; } = null!;

}
