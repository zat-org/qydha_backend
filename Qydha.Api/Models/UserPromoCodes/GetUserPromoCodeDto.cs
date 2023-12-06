namespace Qydha.API.Models;

public class GetUserPromoCodeDto
{
    public Guid Id { get; set; }
    public int NumberOfDays { get; set; }
    public string Code { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }

}
