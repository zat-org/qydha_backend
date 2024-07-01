
namespace Qydha.API.Controllers;

[ApiController]
[Route("notifications/")]
[Authorize(Roles = RoleConstants.Admin)]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet("public/")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicNotifications([FromQuery] PaginationParameters pageParams)
    {
        return (await _notificationService.GetAllAnonymous(pageParams))
        .Resolve(
            (notifications) =>
            {
                return Ok(new
                {
                    Data = new NotificationMapper().PageListToNotificationPageDto(notifications),
                    Message = "Notifications Fetched successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [HttpPatch("click/{notificationId}")]
    [AllowAnonymous]
    public async Task<IActionResult> ApplyAnonymousClickOnNotification([FromRoute] int notificationId)
    {
        return (await _notificationService.ApplyAnonymousClick(notificationId))
        .Resolve(
            () => Ok(
                    new
                    {
                        data = new { },
                        message = "Notification Clicked successfully."
                    }), HttpContext.TraceIdentifier);
    }

    [HttpPost("send-to-user/")]
    public IActionResult SendNotificationToUser([FromForm] NotificationSendToUserDto dto)
    {
        Dictionary<string, object> payload = [];
        return Result.Ok()
        .OnSuccessAsync(async () =>
        {
            if (dto.PopUpImage is not null)
                return await _notificationService.UploadNotificationImage(dto.PopUpImage);
            else
                return Result.Ok(new FileData());
        })
        .OnSuccessAsync(async (fileData) =>
        {
            return await _notificationService.SendToUser(dto.UserId, new NotificationData()
            {
                Title = dto.Title,
                Description = dto.Description,
                ActionPath = dto.ActionPath,
                ActionType = dto.ActionType,
                CreatedAt = DateTimeOffset.UtcNow,
                Payload = new() { Image = dto.PopUpImage is not null ? fileData : null },
                TemplateValues = []
            }, templateValues: []);
        })
        .Resolve((user) => Ok(new { message = $"Notification sent to the user with username = '{user.Username}'" }), HttpContext.TraceIdentifier);
    }


    [HttpPost("send-to-all-users/")]
    public IActionResult SendNotificationToAllUsers([FromForm] NotificationSendDto dto)
    {
        Dictionary<string, object> payload = [];
        return Result.Ok()
        .OnSuccessAsync(async () =>
        {
            if (dto.PopUpImage is not null)
                return await _notificationService.UploadNotificationImage(dto.PopUpImage);
            else
                return Result.Ok(new FileData());
        })
        .OnSuccessAsync(async (fileData) =>
        {
            return await _notificationService.SendToAllUsers(new NotificationData()
            {
                Title = dto.Title,
                Description = dto.Description,
                ActionPath = dto.ActionPath,
                ActionType = dto.ActionType,
                CreatedAt = DateTimeOffset.UtcNow,
                Payload = new() { Image = dto.PopUpImage is not null ? fileData : null },
                TemplateValues = []
            }, templateValues: []);
        })
        .Resolve((usersCount) => Ok(new { message = $"Notification sent to the users with count = '{usersCount}'" }), HttpContext.TraceIdentifier);
    }

    [HttpPost("send-to-anonymous-users/")]
    public IActionResult SendNotificationToAnonymousUsers([FromForm] NotificationSendDto dto)
    {
        Dictionary<string, object> payload = [];
        return Result.Ok()
        .OnSuccessAsync(async () =>
        {
            if (dto.PopUpImage is not null)
                return await _notificationService.UploadNotificationImage(dto.PopUpImage);
            else
                return Result.Ok(new FileData());
        })
        .OnSuccessAsync(async (fileData) =>
        {
            return await _notificationService.SendToAnonymousUsers(new NotificationData()
            {
                Title = dto.Title,
                Description = dto.Description,
                ActionPath = dto.ActionPath,
                ActionType = dto.ActionType,
                CreatedAt = DateTimeOffset.UtcNow,
                Payload = new() { Image = dto.PopUpImage is not null ? fileData : null },
                TemplateValues = []
            });
        })
        .Resolve((notification) => Ok(new { message = $"Notification sent to the Anonymous users" }), HttpContext.TraceIdentifier);
    }
}
