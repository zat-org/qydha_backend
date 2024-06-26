﻿
namespace Qydha.API.Controllers;

[ApiController]
[Route("notifications/")]
[Auth(SystemUserRoles.Admin)]

public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet("public/")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
    {
        return (await _notificationService.GetAllAnonymous(pageSize, pageNumber))
        .Handle<IEnumerable<Notification>, IActionResult>(
            (notificationsData) =>
            {
                var mapper = new NotificationMapper();

                return Ok(
                    new
                    {
                        data = new { notifications = notificationsData.Select(n => mapper.NotificationToGetNotificationDto(n)) },
                        message = "Notifications Fetched successfully."
                    });
            }
            , BadRequest
        );
    }

    [HttpPatch("click/{notificationId}")]
    [AllowAnonymous]
    public async Task<IActionResult> ApplyAnonymousClickOnNotification([FromRoute] int notificationId)
    {
        return (await _notificationService.ApplyAnonymousClick(notificationId))
        .Handle<IActionResult>(
            () => Ok(
                    new
                    {
                        data = new { },
                        message = "Notification Clicked successfully."
                    })
            , BadRequest
        );
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
        .Handle<User, IActionResult>((user) =>
            Ok(new { message = $"Notification sent to the user with username = '{user.Username}'" })
        , BadRequest);
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
        .Handle<int, IActionResult>((usersCount) =>
            Ok(new { message = $"Notification sent to the users with count = '{usersCount}'" })
        , BadRequest);
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
        .Handle<Notification, IActionResult>((notification) =>
            Ok(new { message = $"Notification sent to the Anonymous users" })
        , BadRequest);
    }
}
