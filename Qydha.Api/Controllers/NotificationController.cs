
namespace Qydha.API.Controllers;

[ApiController]
[Route("notifications/")]
public class NotificationController(INotificationService notificationService, IOptions<NotificationImageSettings> optionsOfPhoto) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IOptions<NotificationImageSettings> _optionsOfPhoto = optionsOfPhoto;

    [Authorization(AuthZUserType.Admin)]
    [HttpPost("send-to-user/")]
    public async Task<IActionResult> SendNotificationToUser([FromBody] NotificationSendToUserDto notification_request)
    {
        return (await _notificationService.SendToUser(new Notification()
        {
            Title = notification_request.Title!,
            Description = notification_request.Description!,
            ActionPath = notification_request.Action_Path!,
            ActionType = notification_request.Action_Type,
            CreatedAt = DateTime.UtcNow,
            UserId = notification_request.UserId
        }))
        .Handle<User, IActionResult>((user) =>
            Ok(new { message = $"Notification sent to the user with username = '{user.Username}'" })
        , BadRequest);

    }


    [Authorization(AuthZUserType.Admin)]
    [HttpPost("send-to-all-users/")]
    public async Task<IActionResult> SendNotificationToAllUsers([FromBody] NotificationSendDto dto)
    {
        return (await _notificationService.SendToAllUsers(new Notification()
        {
            Title = dto.Title!,
            Description = dto.Description!,
            ActionPath = dto.Action_Path!,
            ActionType = dto.Action_Type,
            CreatedAt = DateTime.UtcNow,
        }))
        .Handle<int, IActionResult>((effected) => Ok(new { Message = $"notification sent to : {effected} users " }), BadRequest);

    }

    [Authorization(AuthZUserType.Admin)]
    [HttpPost("upload-notification-image/")]
    public async Task<IActionResult> UploadNotificationImage([FromForm] IFormFile file)
    {

        var avatarValidator = new NotificationImageValidator(_optionsOfPhoto);
        var validationRes = avatarValidator.Validate(file);

        if (!validationRes.IsValid)
        {
            return BadRequest(new Error()
            {
                Code = ErrorType.InvalidBodyInput,
                Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
            });
        }

        return (await _notificationService.UploadNotificationImage(file))
        .Handle<FileData, IActionResult>((imageUrl) =>
            {
                return Ok(new
                {
                    data = imageUrl,
                    message = "Image updated successfully."
                });
            }, BadRequest);
    }

}
