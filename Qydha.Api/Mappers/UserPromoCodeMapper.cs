namespace Qydha.API.Mappers;

[Mapper]
public partial class UserPromoCodeMapper
{

    [MapProperty(nameof(UserPromoCode.Expire_At), nameof(GetUserPromoCodeDto.ExpireAt))]
    [MapProperty(nameof(UserPromoCode.Number_Of_Days), nameof(GetUserPromoCodeDto.NumberOfDays))]
    [MapProperty(nameof(UserPromoCode.Created_At), nameof(GetUserPromoCodeDto.CreatedAt))]

    public partial GetUserPromoCodeDto PromoCodeToGetPromoCodeDto(UserPromoCode promo);
}
