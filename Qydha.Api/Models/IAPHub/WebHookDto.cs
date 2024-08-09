namespace Qydha.API.Models;

public class WebHookDto
{

    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTimeOffset CreateDate { get; set; }
    public WebhookData? Data { get; set; }
    public Guid? OldUserId { get; set; }
    public Guid? NewUserId { get; set; }

    public override string ToString()
    {
        return
        @$" 
        Webhook type : {Type}   
        Webhook id : {Id}   
        CreateDate : {CreateDate}
        OldUserId : {OldUserId}
        NewUserId : {NewUserId}
        Data : 
            userId : {Data?.UserId}
            PurchaseId : {Data?.Id}
            PurchaseDate : {Data?.PurchaseDate}
            ProductSku : {Data?.ProductSku}
        ";
    }
}


public class WebhookData
{
    public string Id { get; set; } = null!;
    public DateTimeOffset PurchaseDate { get; set; }
    public DateTimeOffset RefundDate { get; set; }
    public string ProductSku { get; set; } = null!;
    public Guid UserId { get; set; }
    public bool IsSandbox { get; set; }

}