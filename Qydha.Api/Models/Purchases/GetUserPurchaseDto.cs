namespace Qydha.API.Models;
public class GetUserPurchaseDto
{
    public string IAPHubPurchaseId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTimeOffset PurchaseDate { get; set; }
    public string ProductSku { get; set; } = null!;
    public int NumberOfDays { get; set; }
}
