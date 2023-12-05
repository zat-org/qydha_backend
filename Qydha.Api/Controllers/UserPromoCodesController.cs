namespace Qydha.API.Controllers;

[ApiController]
[Route("promo-codes/")]
[Authorize]
[TypeFilter(typeof(AuthFilter))]
public class UserPromoCodesController(IUserPromoCodesService userPromoCodesService) : ControllerBase
{
    #region injections and ctor
    private readonly IUserPromoCodesService _userPromoCodesService = userPromoCodesService;

    #endregion


    [HttpPost]
    public async Task<IActionResult> AddPromo(PromoCodesAddingDto dto)
    {
        return (await _userPromoCodesService.AddPromoCode(dto.UserId, dto.Code, dto.NumberOfDays, dto.ExpireAt))
        .Handle<UserPromoCode, IActionResult>((promo) => Ok(new
        {
            Data = new
            {
                code = promo.Code,
                expireAt = promo.Expire_At,
                numberOfDays = promo.Number_Of_Days
            },
            message = "Promo Added Successfully."
        }), BadRequest);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUserPromoCodes()
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userPromoCodesService.GetUserPromoCodes(userId))
        .Handle<IEnumerable<UserPromoCode>, IActionResult>((promoCodes) =>
        {
            var mapper = new UserPromoCodeMapper();
            return Ok(new
            {
                Data = new { promoCodes = promoCodes.Select(promo => mapper.PromoCodeToGetPromoCodeDto(promo)) },
                message = "Promo Added Successfully."
            });
        }
        , BadRequest);
    }
    [HttpPost("use")]
    public async Task<IActionResult> UsePromo(PromoCodesUsingDto dto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _userPromoCodesService.UsePromoCode(userId, dto.PromoCodeId))
        .Handle<User, IActionResult>((user) =>
        {
            var mapper = new UserMapper();

            return Ok(new
            {
                Data = new { user = mapper.UserToUserDto(user) }
                ,
                message = "Promo Used Successfully."
            });
        }, BadRequest);
    }

}

