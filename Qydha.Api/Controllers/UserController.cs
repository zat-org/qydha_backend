namespace Qydha.API.Controllers;

[ApiController]
[Route("users/")]
public class UserController : ControllerBase
{
    #region injections and ctor
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IOptions<AvatarSettings> _optionsOfPhoto;

    public UserController(IUserService userService, INotificationService notificationService, IOptions<AvatarSettings> optionsOfPhoto)
    {
        _userService = userService;
        _notificationService = notificationService;
        _optionsOfPhoto = optionsOfPhoto;
    }

    #endregion

    #region Get user 
    [HttpGet("me/")]
    [Authorization(AuthZUserType.User)]
    public async Task<IActionResult> GetUser()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.GetUserById(user.Id))
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

    [HttpGet("is-username-available")]
    public async Task<IActionResult> IsUserNameAvailable([FromBody] string username)
    {
        return (await _userService.IsUserNameAvailable(username))
        .Handle<IActionResult>(
            () =>
            {
                return Ok(new
                {
                    data = new { IsAvailable = true },
                    message = "usernames is available."
                });
            },
            BadRequest
        );
    }

    #endregion

    #region update user
    [HttpPatch("me/update-password/")]
    [Authorization(AuthZUserType.User)]
    public async Task<IActionResult> UpdateAuthorizedUserPassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.UpdateUserPassword(user.Id, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
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

    [Authorization(AuthZUserType.User)]
    [HttpPatch("me/update-password-from-phone-authentication/")]
    public async Task<IActionResult> UpdatePhoneAuthorizedUserPassword([FromBody] UpdatePasswordFromPhoneAuthentication dto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.UpdateUserPassword(user.Id, dto.RequestId, dto.NewPassword))
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

    [Authorization(AuthZUserType.User)]
    [HttpPatch("me/update-username/")]
    public async Task<IActionResult> UpdateAuthorizedUsername([FromBody] ChangeUsernameDto changeUsernameDto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.UpdateUserUsername(user.Id, changeUsernameDto.Password, changeUsernameDto.NewUsername))
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

    [Authorization(AuthZUserType.User)]
    [HttpPatch("me/update-phone/")]
    public async Task<IActionResult> UpdateAuthorizedPhone([FromBody] ChangePhoneDto changePhoneDto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.UpdateUserPhone(user.Id, changePhoneDto.Password, changePhoneDto.NewPhone))
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

    [Authorization(AuthZUserType.User)]
    [HttpPost("me/confirm-phone-update/")]
    public async Task<IActionResult> ConfirmPhoneUpdate([FromBody] ConfirmPhoneDto confirmPhoneDto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userService.ConfirmPhoneUpdate(user.Id, confirmPhoneDto.Code, confirmPhoneDto.RequestId))
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

    [Authorization(AuthZUserType.User)]
    [HttpPatch("me/update-email")]
    public async Task<IActionResult> UpdateAuthorizedEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userService.UpdateUserEmail(user.Id, changeEmailDto.Password, changeEmailDto.NewEmail))
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
    [Authorization(AuthZUserType.User)]
    [HttpPost("me/confirm-email-update/")]
    public async Task<IActionResult> ConfirmEmailUpdate([FromBody] ConfirmEmailDto confirmEmailDto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userService.ConfirmEmailUpdate(user.Id, confirmEmailDto.Code, confirmEmailDto.RequestId))
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
    [Authorization(AuthZUserType.User)]

    [HttpPatch("me/update-avatar")]
    public async Task<IActionResult> UpdateUserAvatar([FromForm] IFormFile file)
    {
        User user = (User)HttpContext.Items["User"]!;

        var avatarValidator = new AvatarValidator(_optionsOfPhoto);
        var validationRes = avatarValidator.Validate(file);

        if (!validationRes.IsValid)
        {
            return BadRequest(new Error()
            {
                Code = ErrorType.InvalidBodyInput,
                Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
            });
        }

        return (await _userService.UploadUserPhoto(user.Id, file))
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

    [Authorization(AuthZUserType.User)]

    [HttpPatch("me/update-fcm-token")]
    public async Task<IActionResult> UpdateUsersFCMToken([FromBody] ChangeUserFCMTokenDto changeUserFCMTokenDto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.UpdateFCMToken(user.Id, changeUserFCMTokenDto.FCM_Token))
        .Handle<IActionResult>(() => Ok(new { data = new { }, Message = "User fcm token Updated Successfully" }), BadRequest);
    }


    [Authorization(AuthZUserType.User)]
    [HttpPatch("me/")]
    public async Task<IActionResult> UpdateUserData([FromBody] JsonPatchDocument<UpdateUserDto> updateUserDtoPatch)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.GetUserById(user.Id))
        .OnSuccess<User>((user) =>
        {
            if (updateUserDtoPatch is null)
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = ".لا يوجد بيانات لتحديثها"
                });
            var dto = new UpdateUserDto()
            {
                Name = user.Name ?? string.Empty,
                BirthDate = user.BirthDate ?? null
            };
            updateUserDtoPatch.ApplyTo(dto);
            var validator = new UpdateUserDtoValidator();
            var validationRes = validator.Validate(dto);
            if (!validationRes.IsValid)
            {
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
                });
            }

            user.Name = dto.Name;
            user.BirthDate = dto.BirthDate;
            return Result.Ok(user);
        })
        .OnSuccessAsync<User>(async (user) => await _userService.UpdateUser(user))
        .Handle<User, IActionResult>((user) =>
        {
            var mapper = new UserMapper();
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user) },
                message = "User updated Successfully"
            });
        }, BadRequest);
    }

    #endregion

    #region Delete user
    [Authorization(AuthZUserType.User)]
    [HttpDelete("me/")]
    public async Task<IActionResult> DeleteUser(DeleteUserDto deleteUserDto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userService.DeleteUser(user.Id, deleteUserDto.Password))
        .Handle<User, IActionResult>(
            (user) => Ok(new { data = new { }, message = $"User with username: '{user.Username}' Deleted Successfully." })
            , BadRequest);
    }

    [Authorization(AuthZUserType.User)]
    [HttpDelete("me/delete-anonymous")]
    public async Task<IActionResult> DeleteAnonymousUser()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.DeleteAnonymousUser(user.Id))
        .Handle<User, IActionResult>(
            (user) => Ok(new { data = new { }, message = $"Anonymous user deleted successfully." })
            , BadRequest);
    }

    #endregion

    #region user notifications
    [Authorization(AuthZUserType.User)]
    [HttpGet("me/notifications")]
    public async Task<IActionResult> GetUserNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, [FromQuery] bool? isRead = null)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _notificationService.GetAllNotificationsOfUserById(user.Id, pageSize, pageNumber, isRead))
        .Handle<IEnumerable<Notification>, IActionResult>((notifications) =>
            {
                var mapper = new NotificationMapper();

                return Ok(
                    new
                    {
                        data = new { notifications = notifications.Select(n => mapper.NotificationToGetNotificationDto(n)) },
                        message = "Notifications Fetched successfully."
                    });
            }
         , BadRequest);
    }

    [Authorization(AuthZUserType.User)]
    [HttpPatch("me/notifications/{notificationId}/mark-as-read/")]
    public async Task<IActionResult> MarkNotificationAsRead([FromRoute] int notificationId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.MarkNotificationAsRead(user.Id, notificationId))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "notification marked as read." }), BadRequest);
    }
    [Authorization(AuthZUserType.User)]
    [HttpDelete("me/notifications/{notificationId}")]
    public async Task<IActionResult> DeleteNotification([FromRoute] int notificationId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.DeleteNotification(user.Id, notificationId))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "notification Deleted." }), BadRequest);
    }
    [Authorization(AuthZUserType.User)]
    [HttpDelete("me/notifications/")]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.DeleteAll(user.Id))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "All Notifications has been Deleted." }), BadRequest);
    }
    #endregion
}
