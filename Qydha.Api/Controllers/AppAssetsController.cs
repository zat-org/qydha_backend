namespace Qydha.API.Controllers;

[ApiController]
[Route("/assets")]

public class AppAssetsController(IAppAssetsService appAssetsService) : ControllerBase
{
    private readonly IAppAssetsService _appAssetsService = appAssetsService;


    [Auth(SystemUserRoles.Admin)]
    [HttpPatch("baloot-book/")]
    public async Task<IActionResult> UpdateBalootBook([FromForm] UpdateBalootBookDto dto)
    {
        return (await _appAssetsService.UpdateBalootBookData(dto.File))
        .Resolve((bookAsset) =>
            {
                return Ok(new
                {
                    data = new { book = bookAsset },
                    message = "Baloot Book updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.Admin | SystemUserRoles.RegularUser)]
    [HttpGet("baloot-book/")]
    public async Task<IActionResult> GetBalootBook()
    {
        return (await _appAssetsService.GetBalootBookData())
        .Resolve((bookAsset) =>
            {
                return Ok(new
                {
                    data = new { bookAsset.Url, bookAsset.LastUpdateAt },
                    message = "Baloot Book Fetched successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPatch("popup/")]
    public async Task<IActionResult> UpdatePopupData([FromBody] JsonPatchDocument<PopupDto> popupDtoPatch)
    {
        var mapper = new AssetsMapper();
        return (await _appAssetsService.GetPopupAssetData())
        .OnSuccess((popupAsset) =>
        {
            var dto = mapper.PopUpAssetToPopupDto(popupAsset);
            return popupDtoPatch.ApplyToAsResult(dto)
            .OnSuccess((dtoWithChanges) =>
            {
                var validator = new PopupDtoValidator();
                return validator.ValidateAsResult(dtoWithChanges);
            })
            .OnSuccess((dtoWithChanges) =>
            {
                mapper.PopupDtoToPopUpAsset(dtoWithChanges, popupAsset);
                return Result.Ok(popupAsset);
            });
        })
        .OnSuccessAsync(async (popupAsset) => (await _appAssetsService.UpdatePopupData(popupAsset)).ToResult(popupAsset))
        .Resolve((popupAsset) =>
        {
            return Ok(new
            {
                data = mapper.PopUpAssetToGetPopupDto(popupAsset),
                message = "Popup updated successfully."
            });
        }, HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPut("popup/image")]
    public async Task<IActionResult> UpdatePopupImage([FromForm] UpdatePopupImageDto dto)
    {
        return (await _appAssetsService.UpdatePopupImage(dto.File))
        .Resolve((popupAsset) =>
            {
                var mapper = new AssetsMapper();

                return Ok(new
                {
                    data = mapper.PopUpAssetToGetPopupDto(popupAsset),
                    message = "popup image updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }


    [HttpGet("popup/")]
    public async Task<IActionResult> GetPopup()
    {
        var mapper = new AssetsMapper();

        return (await _appAssetsService.GetPopupAssetData())
        .Resolve((popupAsset) =>
            {
                return Ok(new
                {
                    data = mapper.PopUpAssetToGetPopupDto(popupAsset),
                    message = "Popup Fetched successfully."
                });
            }, HttpContext.TraceIdentifier);
    }
}
