namespace Qydha.Domain.Entities;

[Table("purchases")]
[NotFoundError(ErrorType.PurchaseNotFound)]

public class Purchase : DbEntity<Purchase>
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("iaphub_purchase_id")]
    public string IAPHubPurchaseId { get; set; } = string.Empty;
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("type")]
    public string Type { get; set; } = string.Empty;
    [Column("purchase_date")]
    public DateTime PurchaseDate { get; set; }
    [Column("productsku")]
    public string ProductSku { get; set; } = string.Empty;
    [Column("number_of_days")]
    public int NumberOfDays { get; set; }
    public Purchase()
    {

    }

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
