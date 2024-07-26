namespace Qydha.API.Models;
public class UserInfluencerCodeDto
{
    public Guid InfluencerCodeId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UsedAt { get; set; }
    public DateTimeOffset ExpireAt { get; set; }
    public int NumberOfDays { get; set; }
    public string InfluencerCodeName { get; set; } = null!;
    public UserInfluencerCodeCategoryDto? Category { get; set; }

}

public class UserInfluencerCodeCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}