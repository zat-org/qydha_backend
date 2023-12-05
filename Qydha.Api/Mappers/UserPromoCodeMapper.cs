namespace Qydha.API.Mappers;

[Mapper]
public partial class UserPromoCodeMapper
{

    [MapProperty(nameof(UserPromoCode.Expire_At), nameof(GetUserPromoCodeDto.ExpireAt))]
    [MapProperty(nameof(UserPromoCode.Number_Of_Days), nameof(GetUserPromoCodeDto.NumberOfDays))]
    public partial GetUserPromoCodeDto PromoCodeToGetPromoCodeDto(UserPromoCode promo);
}
