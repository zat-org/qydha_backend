namespace Qydha.API.Controllers;

[ApiController]
[Route("/assets")]

public class AppAssetsController(IAppAssetsService appAssetsService, IOptions<BookSettings> bookOptions, IOptions<NotificationImageSettings> notificationImageOptions) : ControllerBase
{
    private readonly IAppAssetsService _appAssetsService = appAssetsService;
    private readonly IOptions<BookSettings> _bookOptions = bookOptions;
    private readonly IOptions<NotificationImageSettings> _notificationImageOptions = notificationImageOptions;



    [Auth(SystemUserRoles.Admin)]
    [HttpPatch("baloot-book/")]
    public async Task<IActionResult> UpdateBalootBook([FromForm] IFormFile file)
    {
        var bookValidator = new BookValidator(_bookOptions);
        var validationRes = bookValidator.Validate(file);

        if (!validationRes.IsValid)
        {
            return BadRequest(new Error()
            {
                Code = ErrorType.InvalidBodyInput,
                Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
            });
        }
        return (await _appAssetsService.UpdateBalootBookData(file))
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

        var mapper = new AssetsMapper();

        return (await _appAssetsService.GetPopupAssetData())
        .OnSuccess<PopUpAsset>((popupAsset) =>
        {
            if (popupDtoPatch is null)
                return Result.Fail<PopUpAsset>(new()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = "لا يوجد بيانات لتحديثها"
                });
            var dto = mapper.PopUpAssetToPopupDto(popupAsset);
            try
            {
                popupDtoPatch.ApplyTo(dto);
            }
            catch (JsonPatchException exp)
            {
                return Result.Fail<PopUpAsset>(new()
                {
                    Code = ErrorType.InvalidPatchBodyInput,
                    Message = exp.Message
                });
            }
            var validator = new PopupDtoValidator();
            var validationRes = validator.Validate(dto);

            if (!validationRes.IsValid)
                return Result.Fail<PopUpAsset>(new Error()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
                });

            if (dto.Show && popupAsset.Image is null)
                return Result.Fail<PopUpAsset>(new Error()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = "لا يمكن تحويل حالة الاعلان الي  ظاهر وهو بدون صورة"
                });

            popupAsset.Show = dto.Show;
            popupAsset.ActionPath = dto.ActionPath;
            popupAsset.ActionType = dto.ActionType;

            return Result.Ok(popupAsset);
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
    public async Task<IActionResult> UpdatePopupImage([FromForm] IFormFile file)
    {
        var imageValidator = new NotificationImageValidator(_notificationImageOptions);
        var validationRes = imageValidator.Validate(file);

        if (!validationRes.IsValid)
        {
            return BadRequest(new Error()
            {
                Code = ErrorType.InvalidBodyInput,
                Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
            });
        }
        return (await _appAssetsService.UpdatePopupImage(file))
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
