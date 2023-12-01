
namespace Qydha.API.Controllers;

[Authorize]
[TypeFilter(typeof(AuthFilter))]
[Route("notifications/")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public NotificationController(IUserService userService, INotificationService notificationService)
    {
        _notificationService = notificationService;
        _userService = userService;
    }

    [HttpPost("send-to-user/")]
    public async Task<IActionResult> SendNotificationToUser([FromBody] NotificationSendToUserDto notification_request)
    {
        return (await _notificationService.SendToUser(new Notification()
        {
            Title = notification_request.Title!,
            Description = notification_request.Description!,
            Action_Path = notification_request.Action_Path!,
            Action_Type = notification_request.Action_Type,
            Created_At = DateTime.Now,
            User_Id = notification_request.UserId
        }))
        .Handle<User, IActionResult>((user) =>
            Ok(new { message = $"Notification sent to the user with username = '{user.Username}'" })
        , BadRequest);

    }

    [HttpPost("send-to-all-users/")]
    public async Task<IActionResult> SendNotificationToAllUsers([FromBody] NotificationSendDto dto)
    {
        return (await _notificationService.SendToAllUsers(new Notification()
        {
            Title = dto.Title!,
            Description = dto.Description!,
            Action_Path = dto.Action_Path!,
            Action_Type = dto.Action_Type,
            Created_At = DateTime.Now,
        }))
        .Handle<int, IActionResult>((effected) => Ok(new { Message = $"notification sent to : {effected} users " }), BadRequest);

    }

}
