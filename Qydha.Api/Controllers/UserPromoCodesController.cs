namespace Qydha.API.Controllers;

[ApiController]
[Route("promo-codes/")]
public class UserPromoCodesController(IUserPromoCodesService userPromoCodesService) : ControllerBase
{
    #region injections and ctor
    private readonly IUserPromoCodesService _userPromoCodesService = userPromoCodesService;

    #endregion


    [HttpPost]
    [Auth(SystemUserRoles.Admin)]
    public async Task<IActionResult> AddPromo(PromoCodesAddingDto dto)
    {
        return (await _userPromoCodesService.AddPromoCode(dto.UserId, dto.Code, dto.NumberOfDays, dto.ExpireAt))
        .Resolve((promo) => Ok(new
        {
            Data = new
            {
                code = promo.Code,
                expireAt = promo.ExpireAt,
                numberOfDays = promo.NumberOfDays
            },
            message = "Promo Code Added Successfully."
        }), HttpContext.TraceIdentifier);
    }

    [HttpGet]
    [Auth(SystemUserRoles.RegularUser)]

    public async Task<IActionResult> GetAllUserPromoCodes()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userPromoCodesService.GetUserValidPromoCodeAsync(user.Id))
        .Resolve((promoCodes) =>
        {
            var mapper = new UserPromoCodeMapper();
            return Ok(new
            {
                Data = new { promoCodes = promoCodes.Select(promo => mapper.PromoCodeToGetPromoCodeDto(promo)) },
                message = "Promo Code Fetched Successfully."
            });
        }, HttpContext.TraceIdentifier);
    }

    [HttpPost("use")]
    [Auth(SystemUserRoles.RegularUser)]

    public async Task<IActionResult> UsePromo(PromoCodesUsingDto dto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userPromoCodesService.UsePromoCode(user.Id, dto.PromoCodeId))
        .Resolve((user) =>
        {
            var mapper = new UserMapper();

            return Ok(new
            {
                Data = new { user = mapper.UserToUserDto(user) }
                ,
                message = "Promo Code Used Successfully."
            });
        }, HttpContext.TraceIdentifier);
    }

}

