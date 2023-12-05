namespace Qydha.API.Models;

public class GetUserPromoCodeDto
{
    public int NumberOfDays { get; set; }
    public string Code { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
}
