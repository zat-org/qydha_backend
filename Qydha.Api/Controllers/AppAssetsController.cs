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
        .Handle<BookAsset, IActionResult>((bookAsset) =>
            {
                return Ok(new
                {
                    data = new { book = bookAsset },
                    message = "Baloot Book updated successfully."
                });
            },
            BadRequest);
    }

    [Auth(SystemUserRoles.Admin | SystemUserRoles.RegularUser)]
    [HttpGet("baloot-book/")]
    public async Task<IActionResult> GetBalootBook()
    {
        return (await _appAssetsService.GetBalootBookData())
        .Handle<BookAsset, IActionResult>((bookAsset) =>
            {
                return Ok(new
                {
                    data = new { bookAsset.Url, bookAsset.LastUpdateAt },
                    message = "Baloot Book Fetched successfully."
                });
            },
            BadRequest);
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPatch("popup/")]
    public async Task<IActionResult> UpdatePopupData([FromBody] JsonPatchDocument<PopupDto> popupDtoPatch)
    {
        if (popupDtoPatch is null)
            return BadRequest(new Error()
            {
                Code = ErrorType.InvalidBodyInput,
                Message = "لا يوجد بيانات لتحديثها"
            });

        var mapper = new AssetsMapper();

        return (await _appAssetsService.GetPopupAssetData())
        .OnSuccess<PopUpAsset>((popupAsset) =>
        {
            var dto = mapper.PopUpAssetToPopupDto(popupAsset);
            return popupDtoPatch.ApplyToAsResult(dto)
            .OnSuccess<PopupDto>((dtoWithChanges) =>
            {
                var validator = new PopupDtoValidator();
                return validator.ValidateAsResult(dtoWithChanges);
            })
            .OnSuccess<PopupDto>((dtoWithChanges) =>
            {
                if (dtoWithChanges.Show && popupAsset.Image is null)
                    return Result.Fail<PopupDto>(new Error()
                    {
                        Code = ErrorType.InvalidBodyInput,
                        Message = "لا يمكن تحويل حالة الاعلان الي  ظاهر وهو بدون صورة"
                    });
                else
                    return Result.Ok(dtoWithChanges);
            })
            .OnSuccess((dtoWithChanges) =>
            {
                mapper.PopupDtoToPopUpAsset(dto, popupAsset);
                return Result.Ok(popupAsset);
            });

        })
        .OnSuccessAsync<PopUpAsset>(async (popupAsset) => (await _appAssetsService.UpdatePopupData(popupAsset)).MapTo(popupAsset))
        .Handle<PopUpAsset, IActionResult>((popupAsset) =>
            {
                return Ok(new
                {
                    data = mapper.PopUpAssetToGetPopupDto(popupAsset),
                    message = "Popup updated successfully."
                });
            },
            BadRequest);
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPut("popup/image")]
    public async Task<IActionResult> UpdatePopupImage([FromForm] UpdatePopupImageDto dto)
    {
        return (await _appAssetsService.UpdatePopupImage(dto.File))
        .Handle<PopUpAsset, IActionResult>((popupAsset) =>
            {
                var mapper = new AssetsMapper();

                return Ok(new
                {
                    data = mapper.PopUpAssetToGetPopupDto(popupAsset),
                    message = "popup image updated successfully."
                });
            },
            BadRequest);
    }


    [HttpGet("popup/")]
    public async Task<IActionResult> GetPopup()
    {
        var mapper = new AssetsMapper();

        return (await _appAssetsService.GetPopupAssetData())
        .Handle<PopUpAsset, IActionResult>((popupAsset) =>
            {
                return Ok(new
                {
                    data = mapper.PopUpAssetToGetPopupDto(popupAsset),
                    message = "Popup Fetched successfully."
                });
            },
            BadRequest);
    }
}
