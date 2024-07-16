namespace Qydha.API.Mappers;

[Mapper]
public partial class PurchasesMapper
{

    [MapperIgnoreSource(nameof(UserPromoCode.UserId))]
    [MapperIgnoreSource(nameof(UserPromoCode.Id))]
    [MapperIgnoreSource(nameof(UserPromoCode.User))]
    public partial GetUserPurchaseDto PurchaseToGetUserPurchaseDto(Purchase purchase);


}
