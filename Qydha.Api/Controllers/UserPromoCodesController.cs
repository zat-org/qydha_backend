namespace Qydha.API.Controllers;

[ApiController]
[Route("promo-codes/")]
public class UserPromoCodesController(IUserPromoCodesService userPromoCodesService) : ControllerBase
{
    #region injections and ctor
    private readonly IUserPromoCodesService _userPromoCodesService = userPromoCodesService;

    #endregion


    [HttpPost]
    [Authorization(AuthZUserType.Admin)]
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
            message = "Promo Code Added Successfully."
        }), BadRequest);
    }

    [HttpGet]
    [Authorization(AuthZUserType.User)]

    public async Task<IActionResult> GetAllUserPromoCodes()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userPromoCodesService.GetUserPromoCodes(user.Id))
        .Handle<IEnumerable<UserPromoCode>, IActionResult>((promoCodes) =>
        {
            var mapper = new UserPromoCodeMapper();
            return Ok(new
            {
                Data = new { promoCodes = promoCodes.Select(promo => mapper.PromoCodeToGetPromoCodeDto(promo)) },
                message = "Promo Code Fetched Successfully."
            });
        }
        , BadRequest);
    }

    [HttpPost("use")]
    [Authorization(AuthZUserType.User)]

    public async Task<IActionResult> UsePromo(PromoCodesUsingDto dto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userPromoCodesService.UsePromoCode(user.Id, dto.PromoCodeId))
        .Handle<User, IActionResult>((user) =>
        {
            var mapper = new UserMapper();

            return Ok(new
            {
                Data = new { user = mapper.UserToUserDto(user) }
                ,
                message = "Promo Code Used Successfully."
            });
        }, BadRequest);
    }

}

