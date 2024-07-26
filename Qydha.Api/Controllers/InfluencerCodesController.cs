namespace Qydha.API.Controllers;

[ApiController]
[Route("influencer-codes/")]
public class InfluencerCodesController(IInfluencerCodesService influencerCodesService) : ControllerBase
{
    private readonly IInfluencerCodesService _influencerCodesService = influencerCodesService;

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
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
    [Authorize(Roles = RoleConstants.User)]
    public IActionResult UseInfluencerCode(UseInfluencerCodeDto dto)
    {

        return HttpContext.User.GetUserIdentifier()
        .OnSuccessAsync(async id => await _influencerCodesService.UseInfluencerCode(id, dto.Code))
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
