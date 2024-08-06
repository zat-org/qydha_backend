namespace Qydha.API.Models;

public class GetUserPromoCodeDto
{
    public Guid Id { get; set; }
    public int NumberOfDays { get; set; }
    public string Code { get; set; } = null!;
    public DateTimeOffset ExpireAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

}
public class GetUsedPromoCodeByUserDto : GetUserPromoCodeDto
{
    public DateTimeOffset? UsedAt { get; set; }
}
