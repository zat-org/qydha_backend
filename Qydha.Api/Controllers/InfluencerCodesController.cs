namespace Qydha.API.Controllers;

[ApiController]
[Route("influencer-codes/")]
public class InfluencerCodesController(IInfluencerCodesService influencerCodesService) : ControllerBase
{
    private readonly IInfluencerCodesService _influencerCodesService = influencerCodesService;

    [HttpPost]
    [Auth(SystemUserRoles.Admin)]
    public async Task<IActionResult> AddInfluencerCode(AddInfluencerCodeDto dto)
    {
        return (await _influencerCodesService.AddInfluencerCode(dto.Code, dto.NumberOfDays, dto.ExpireAt, dto.MaxInfluencedUsersCount, dto.CategoryId))
        .Resolve((infCode) => Ok(new
        {
            Data = new
            {
                code = infCode.Code,
                expireAt = infCode.ExpireAt,
                numberOfDays = infCode.NumberOfDays,
                categoryId = infCode.CategoryId
            },
            message = "Influencer Code Added Successfully."
        }), HttpContext.TraceIdentifier);
    }

    [HttpPost("use")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> UseInfluencerCode(UseInfluencerCodeDto dto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _influencerCodesService.UseInfluencerCode(user.Id, dto.Code))
        .Resolve((user) =>
        {
            var mapper = new UserMapper();

            return Ok(new
            {
                Data = new { user = mapper.UserToUserDto(user) },
                message = "Influencer Code Used Successfully."
            });
        }, HttpContext.TraceIdentifier);
    }

}
