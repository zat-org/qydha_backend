namespace Qydha.API.Controllers;

[ApiController]
[Route("users/")]
[Authorize]
[TypeFilter(typeof(AuthFilter))]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IOptions<AvatarSettings> _optionsOfPhoto;

    public UserController(IUserService userService, INotificationService notificationService, IOptions<AvatarSettings> optionsOfPhoto)
    {
        _userService = userService;
        _notificationService = notificationService;
        _optionsOfPhoto = optionsOfPhoto;
    }
    [HttpGet("me/")]
    public async Task<IActionResult> GetUser()
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.GetUserById(userId))
        .Handle<User, IActionResult>(
            (user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User fetched successfully."
                });
            },
            NotFound
        );
    }

    [HttpPatch("me/update-password/")]
    public async Task<IActionResult> UpdateAuthorizedUserPassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.UpdateUserPassword(userId, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
        .Handle<User, IActionResult>((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            },
            BadRequest);
    }


    [HttpPatch("me/update-username/")]
    public async Task<IActionResult> UpdateAuthorizedUsername([FromBody] ChangeUsernameDto changeUsernameDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.UpdateUserUsername(userId, changeUsernameDto.Password, changeUsernameDto.NewUsername))
        .Handle<User, IActionResult>((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            },
            BadRequest);
    }

    [HttpPatch("me/update-phone/")]
    public async Task<IActionResult> UpdateAuthorizedPhone([FromBody] ChangePhoneDto changePhoneDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.UpdateUserPhone(userId, changePhoneDto.Password, changePhoneDto.NewPhone))
        .Handle<UpdatePhoneRequest, IActionResult>((otp_request) =>
            {
                return Ok(new
                {
                    Data = new { RequestId = otp_request.Id },
                    Message = "Otp sent successfully."
                });
            },
            BadRequest);
    }

    [HttpPost("me/confirm-phone-update/")]
    public async Task<IActionResult> ConfirmPhoneUpdate([FromBody] ConfirmPhoneDto confirmPhoneDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _userService.ConfirmPhoneUpdate(userId, confirmPhoneDto.Code, confirmPhoneDto.RequestId))
        .Handle<User, IActionResult>((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            },
            BadRequest);
    }

    [HttpPatch("me/update-email")]
    public async Task<IActionResult> UpdateAuthorizedEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _userService.UpdateUserEmail(userId, changeEmailDto.Password, changeEmailDto.NewEmail))
        .Handle<UpdateEmailRequest, IActionResult>((otp_request) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    Data = new { RequestId = otp_request.Id },
                    Message = "User updated successfully."
                });
            },
            BadRequest);
    }


    [HttpGet("me/confirm-email-update/")]
    public async Task<IActionResult> ConfirmEmailUpdate([FromBody] ConfirmEmailDto confirmEmailDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _userService.ConfirmEmailUpdate(userId, confirmEmailDto.Code, confirmEmailDto.RequestId))
        .Handle<User, IActionResult>((user) =>
        {
            var mapper = new UserMapper();
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user) },
                message = "User updated successfully."
            });
        },
        BadRequest);
    }

    [HttpPatch("me/update-avatar")]
    public async Task<IActionResult> UpdateUserAvatar([FromForm] IFormFile file)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        var avatarValidator = new AvatarValidator(_optionsOfPhoto);
        var validationRes = avatarValidator.Validate(file);

        if (!validationRes.IsValid)
        {
            return BadRequest(new
            {
                code = ErrorCodes.InvalidBodyInput,
                message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
            });
        }

        return (await _userService.UploadUserPhoto(userId, file))
        .Handle<User, IActionResult>((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            }, BadRequest);
    }

    [HttpPatch("me/")]
    public async Task<IActionResult> UpdateUserData([FromBody] JsonPatchDocument<UpdateUserDto> updateUserDtoPatch)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.GetUserById(userId))
        .OnSuccess<User>((user) =>
        {
            if (updateUserDtoPatch is null)
                return Result.Fail<User>(new()
                {
                    Code = ErrorCodes.InvalidBodyInput,
                    Message = ".لا يوجد بيانات لتحديثها"
                });
            var dto = new UpdateUserDto()
            {
                Name = user.Name ?? string.Empty,
                BirthDate = user.Birth_Date ?? null
            };
            updateUserDtoPatch.ApplyTo(dto);
            var validator = new UpdateUserDtoValidator();
            var validationRes = validator.Validate(dto);
            if (!validationRes.IsValid)
            {
                return Result.Fail<User>(new()
                {
                    Code = ErrorCodes.InvalidBodyInput,
                    Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
                });
            }

            user.Name = dto.Name;
            user.Birth_Date = dto.BirthDate;
            return Result.Ok(user);
        })
        .OnSuccessAsync<User>(async (user) => await _userService.UpdateUser(user))
        .Handle<User, IActionResult>((user) =>
        {
            var mapper = new UserMapper();
            return Ok(new
            {
                data = mapper.UserToUserDto(user),
                message = "User updated Successfully"
            });
        }, BadRequest);
    }

    [HttpDelete("me/")]
    public async Task<IActionResult> DeleteUser(DeleteUserDto deleteUserDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _userService.DeleteUser(userId, deleteUserDto.Password))
        .Handle<User, IActionResult>(
            (user) => Ok(new { message = $"User with username: '{user.Username}' Deleted Successfully." })
            , BadRequest);
    }

    [HttpDelete("me/delete-anonymous")]
    public async Task<IActionResult> DeleteAnonymousUser()
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.DeleteAnonymousUser(userId))
        .Handle<User, IActionResult>(
            (user) => Ok(new { message = $"Anonymous user deleted successfully." })
            , BadRequest);
    }

    [HttpGet("me/notifications")]
    public async Task<IActionResult> GetUserNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, [FromQuery] bool? isRead = null)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _notificationService.GetAllNotificationsOfUserById(userId, pageSize, pageNumber, isRead))
        .Handle<IEnumerable<Notification>, IActionResult>((notifications) =>
            Ok(new
            {
                data = notifications,
                message = "Notifications Fetched successfully."
            })
         , BadRequest);
    }

    [HttpPatch("me/notifications/{notificationId}/mark-as-read/")]
    public async Task<IActionResult> MarkNotificationAsRead([FromRoute] int notificationId)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _notificationService.MarkNotificationAsRead(userId, notificationId))
        .Handle<IActionResult>(() => Ok(new { message = "notification marked as read." }), BadRequest);
    }

    [HttpPatch("me/update-fcm-token")]
    public async Task<IActionResult> UpdateUsersFCMToken([FromBody] ChangeUserFCMTokenDto changeUserFCMTokenDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _userService.UpdateFCMToken(userId, changeUserFCMTokenDto.FCM_Token))
        .Handle<IActionResult>(() => Ok(new { Message = "User fcm token Updated Successfully" }), BadRequest);
    }
}
