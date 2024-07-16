namespace Qydha.API.Mappers;

[Mapper]
public partial class UserPromoCodeMapper
{

    [MapperIgnoreSource(nameof(UserPromoCode.UsedAt))]
    [MapperIgnoreSource(nameof(UserPromoCode.UserId))]
    [MapperIgnoreSource(nameof(UserPromoCode.User))]
    public partial GetUserPromoCodeDto PromoCodeToGetPromoCodeDto(UserPromoCode promo);
    [MapperIgnoreSource(nameof(UserPromoCode.UserId))]
    [MapperIgnoreSource(nameof(UserPromoCode.User))]
    public partial GetUsedPromoCodeByUserDto PromoCodeToGetUsedPromoCodeDto(UserPromoCode promo);

}
