namespace Qydha.Domain.Entities;
public class Purchase
{
    public Guid Id { get; set; }
    public string IAPHubPurchaseId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public int NumberOfDays { get; set; }
    public virtual User User { get; set; } = null!;

    public Purchase() { }

    public Purchase(InfluencerCode influencerCode, Guid userId)
    {
        IAPHubPurchaseId = influencerCode.Id.ToString();
        UserId = userId;
        Type = "Influencer";
        PurchaseDate = DateTime.UtcNow;
        ProductSku = influencerCode.Code;
        NumberOfDays = influencerCode.NumberOfDays;
    }
    public Purchase(UserPromoCode promoCode)
    {
        IAPHubPurchaseId = promoCode.Id.ToString();
        UserId = promoCode.UserId;
        Type = "promo_code";
        PurchaseDate = DateTime.UtcNow;
        ProductSku = promoCode.Code;
        NumberOfDays = promoCode.NumberOfDays;
    }

}
