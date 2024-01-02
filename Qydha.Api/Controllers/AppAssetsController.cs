namespace Qydha.API.Controllers;

[ApiController]
[Route("/assets")]

public class AppAssetsController(IAppAssetsService appAssetsService, IOptions<BookSettings> bookOptions) : ControllerBase
{
    private readonly IAppAssetsService _appAssetsService = appAssetsService;
    private readonly IOptions<BookSettings> _bookOptions = bookOptions;

    [Authorization(AuthZUserType.Admin)]
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

    [Authorization(AuthZUserType.User)]
    [HttpGet("baloot-book/")]
    public async Task<IActionResult> GetBook()
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
}
