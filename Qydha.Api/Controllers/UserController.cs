
namespace Qydha.API.Controllers;

[ApiController]
[Route("users/")]
public class UserController(IUserService userService, INotificationService notificationService, IOptions<AvatarSettings> optionsOfPhoto) : ControllerBase
{
    #region injections and ctor
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IOptions<AvatarSettings> _optionsOfPhoto = optionsOfPhoto;

    #endregion

    #region Get user 
    [HttpGet]
    [Auth(SystemUserRoles.Admin)]
    public async Task<IActionResult> GetUsers()
    {
        return (await _userService.GetAllRegularUsers())
              .Handle<IEnumerable<User>, IActionResult>((users) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { users = users.Select(u => mapper.UserToUserDto(u)) },
                    message = "users fetched successfully."
                });
            }, BadRequest);
    }

    [HttpGet("me/")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> GetUser()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.GetUserWithSettingsByIdAsync(user.Id))
       .Handle<Tuple<User, UserGeneralSettings?, UserHandSettings?, UserBalootSettings?>, IActionResult>(
           (userDataType) =>
           {
               User user = userDataType.Item1;
               var generalSettings = userDataType.Item2;
               var handSettings = userDataType.Item3;
               var balootSettings = userDataType.Item4;

               var mapper = new UserMapper();
               return Ok(new
               {
                   data = new
                   {
                       user = mapper.UserToUserDto(user),
                       generalSettings = generalSettings is null ? null : mapper.UserGeneralSettingsToDto(generalSettings),
                       handSettings = handSettings is null ? null : mapper.UserHandSettingsToDto(handSettings),
                       balootSettings = balootSettings is null ? null : mapper.UserBalootSettingsToDto(balootSettings)
                   },
                   message = "User fetched successfully."
               });
           },
           BadRequest
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
    [Auth(SystemUserRoles.RegularUser)]
    [HttpPatch("me/update-password/")]
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

    [Auth(SystemUserRoles.RegularUser)]
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

    [Auth(SystemUserRoles.RegularUser)]
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

    [Auth(SystemUserRoles.RegularUser)]
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

    [Auth(SystemUserRoles.RegularUser)]
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

    [Auth(SystemUserRoles.RegularUser)]
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
    [Auth(SystemUserRoles.RegularUser)]
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
    [Auth(SystemUserRoles.RegularUser)]

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

    [Auth(SystemUserRoles.RegularUser)]
    [HttpPatch("me/update-fcm-token")]
    public async Task<IActionResult> UpdateUsersFCMToken([FromBody] ChangeUserFCMTokenDto changeUserFCMTokenDto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _userService.UpdateFCMToken(user.Id, changeUserFCMTokenDto.FCMToken))
        .Handle<IActionResult>(() => Ok(new { data = new { }, Message = "User fcm token Updated Successfully" }), BadRequest);
    }


    [Auth(SystemUserRoles.RegularUser)]
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
                    Message = "لا يوجد بيانات لتحديثها"
                });
            var dto = new UpdateUserDto()
            {
                Name = user.Name ?? string.Empty,
                BirthDate = user.BirthDate ?? null
            };
            try
            {
                updateUserDtoPatch.ApplyTo(dto);
            }
            catch (JsonPatchException exp)
            {
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidPatchBodyInput,
                    Message = exp.Message
                });
            }
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
        .OnSuccessAsync<User>(_userService.UpdateUser)
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
    [Auth(SystemUserRoles.RegularUser)]
    [HttpDelete("me/")]
    public async Task<IActionResult> DeleteUser(DeleteUserDto deleteUserDto)
    {
        User user = (User)HttpContext.Items["User"]!;

        return (await _userService.DeleteUser(user.Id, deleteUserDto.Password))
        .Handle<User, IActionResult>(
            (user) => Ok(new { data = new { }, message = $"User with username: '{user.Username}' Deleted Successfully." })
            , BadRequest);
    }

    [HttpDelete("me/delete-anonymous")]
    public IActionResult DeleteAnonymousUser()
    {
        return BadRequest(new Error()
        {
            Code = ErrorType.InvalidBodyInput,
            Message = "برجاء تحديث التطبيق"
        });
    }

    #endregion

    #region users notifications
    [Auth(SystemUserRoles.RegularUser)]
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

    [Auth(SystemUserRoles.RegularUser)]
    [HttpPatch("me/notifications/{notificationId}/mark-as-read/")]
    public async Task<IActionResult> MarkNotificationAsRead([FromRoute] int notificationId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.MarkNotificationAsRead(user.Id, notificationId))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "notification marked as read." }), BadRequest);
    }
    [Auth(SystemUserRoles.RegularUser)]
    [HttpPatch("me/notifications/mark-all-as-read/")]
    public async Task<IActionResult> MarkAllNotificationAsRead([FromRoute] int notificationId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.MarkAllNotificationsOfUserAsRead(user.Id))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "notification marked as read." }), BadRequest);
    }
    [Auth(SystemUserRoles.RegularUser)]
    [HttpDelete("me/notifications/{notificationId}")]
    public async Task<IActionResult> DeleteNotification([FromRoute] int notificationId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.DeleteNotification(user.Id, notificationId))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "notification Deleted." }), BadRequest);
    }
    [Auth(SystemUserRoles.RegularUser)]
    [HttpDelete("me/notifications/")]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _notificationService.DeleteAll(user.Id))
        .Handle<IActionResult>(() => Ok(new { data = new { }, message = "All Notifications has been Deleted." }), BadRequest);
    }
    #endregion

    #region user Settings

    [HttpPatch("me/general-settings")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> UpdateUserGeneralSettings([FromBody] JsonPatchDocument<UserGeneralSettingsDto> generalSettingsDtoPatch)
    {
        User user = (User)HttpContext.Items["User"]!;
        var mapper = new UserMapper();

        return (await _userService.GetUserGeneralSettings(user.Id))
        .OnSuccess<UserGeneralSettings>((settings) =>
        {
            if (generalSettingsDtoPatch is null)
                return Result.Fail<UserGeneralSettings>(new()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = "لا يوجد بيانات لتحديثها"
                });
            var dto = mapper.UserGeneralSettingsToDto(settings);
            try
            {
                generalSettingsDtoPatch.ApplyTo(dto);
            }
            catch (JsonPatchException exp)
            {
                return Result.Fail<UserGeneralSettings>(new()
                {
                    Code = ErrorType.InvalidPatchBodyInput,
                    Message = exp.Message
                });
            }
            // validate here
            settings.PlayersNames = new Json<IEnumerable<string>>(dto.PlayersNames);
            settings.TeamsNames = new Json<IEnumerable<string>>(dto.TeamsNames);
            settings.EnableVibration = dto.EnableVibration;
            return Result.Ok(settings);
        })
        .OnSuccessAsync<UserGeneralSettings>(_userService.UpdateUserGeneralSettings)
        .Handle<UserGeneralSettings, IActionResult>((settings) =>
        {
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user), generalSettings = mapper.UserGeneralSettingsToDto(settings) },
                message = "User's General settings updated successfully."
            });
        }, BadRequest);
    }

    [HttpPatch("me/hand-settings")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> UpdateUserHandSettings([FromBody] JsonPatchDocument<UserHandSettingsDto> handSettingsDtoPatch)
    {
        User user = (User)HttpContext.Items["User"]!;
        var mapper = new UserMapper();

        return (await _userService.GetUserHandSettings(user.Id))
        .OnSuccess<UserHandSettings>((settings) =>
        {
            if (handSettingsDtoPatch is null)
                return Result.Fail<UserHandSettings>(new()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = "لا يوجد بيانات لتحديثها"
                });

            var dto = mapper.UserHandSettingsToDto(settings);
            try
            {
                handSettingsDtoPatch.ApplyTo(dto);
            }
            catch (JsonPatchException exp)
            {
                return Result.Fail<UserHandSettings>(new()
                {
                    Code = ErrorType.InvalidPatchBodyInput,
                    Message = exp.Message
                });
            }

            var handSettingsValidator = new UserHandSettingsDtoValidator();
            var validationRes = handSettingsValidator.Validate(dto);

            if (!validationRes.IsValid)
            {
                return Result.Fail<UserHandSettings>(new Error()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = string.Join(" ;", validationRes.Errors.Select(e => e.ErrorMessage))
                });
            }

            settings.MaxLimit = dto.MaxLimit;
            settings.RoundsCount = dto.RoundsCount;
            settings.PlayersCountInTeam = dto.PlayersCountInTeam;
            settings.TeamsCount = dto.TeamsCount;
            settings.WinUsingZat = dto.WinUsingZat;
            settings.TakweeshPoints = dto.TakweeshPoints;

            return Result.Ok(settings);
        })
        .OnSuccessAsync<UserHandSettings>(_userService.UpdateUserHandSettings)
        .Handle<UserHandSettings, IActionResult>((settings) =>
        {
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user), handSettings = mapper.UserHandSettingsToDto(settings) },
                message = "User's Hand settings updated successfully."
            });
        }, BadRequest);
    }

    [HttpPatch("me/baloot-settings")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> UpdateUserBalootSettings([FromBody] JsonPatchDocument<UserBalootSettingsDto> balootSettingsDtoPatch)
    {
        User user = (User)HttpContext.Items["User"]!;
        var mapper = new UserMapper();

        return (await _userService.GetUserBalootSettings(user.Id))
        .OnSuccess<UserBalootSettings>((settings) =>
        {
            if (balootSettingsDtoPatch is null)
                return Result.Fail<UserBalootSettings>(new()
                {
                    Code = ErrorType.InvalidBodyInput,
                    Message = ".لا يوجد بيانات لتحديثها"
                });

            var dto = mapper.UserBalootSettingsToDto(settings);
            try
            {
                balootSettingsDtoPatch.ApplyTo(dto);
            }
            catch (JsonPatchException exp)
            {
                return Result.Fail<UserBalootSettings>(new()
                {
                    Code = ErrorType.InvalidPatchBodyInput,
                    Message = exp.Message
                });
            }
            // validate here
            settings.IsAdvancedRecording = dto.IsAdvancedRecording;
            settings.IsFlipped = dto.IsFlipped;
            settings.ShowWhoWonDialogOnDraw = dto.ShowWhoWonDialogOnDraw;
            settings.IsSakkahMashdodahMode = dto.IsSakkahMashdodahMode;
            settings.IsNumbersSoundEnabled = dto.IsNumbersSoundEnabled;
            settings.IsCommentsSoundEnabled = dto.IsCommentsSoundEnabled;
            return Result.Ok(settings);
        })
        .OnSuccessAsync<UserBalootSettings>(_userService.UpdateUserBalootSettings)
        .Handle<UserBalootSettings, IActionResult>((settings) =>
        {
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user), balootSettings = mapper.UserBalootSettingsToDto(settings) },
                message = "User's baloot settings updated successfully."
            });
        }, BadRequest);
    }

    [HttpGet("me/general-settings")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> GetUserGeneralSettings()
    {
        User user = (User)HttpContext.Items["User"]!;
        var mapper = new UserMapper();
        return (await _userService.GetUserGeneralSettings(user.Id))
        .Handle<UserGeneralSettings, IActionResult>(
            (settings) =>
            {
                return Ok(new
                {
                    data = new { generalSettings = mapper.UserGeneralSettingsToDto(settings) },
                    message = "Settings fetched successfully."
                });
            }
            , BadRequest);
    }
    [HttpGet("me/hand-settings")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> GetUserHandSettings()
    {
        User user = (User)HttpContext.Items["User"]!;
        var mapper = new UserMapper();
        return (await _userService.GetUserHandSettings(user.Id))
        .Handle<UserHandSettings, IActionResult>(
            (settings) =>
            {
                return Ok(new
                {
                    data = new { handSettings = mapper.UserHandSettingsToDto(settings) },
                    message = "Settings fetched successfully."
                });
            }
            , BadRequest);
    }

    [HttpGet("me/baloot-settings")]
    [Auth(SystemUserRoles.RegularUser)]
    public async Task<IActionResult> GetUserBalootSettings()
    {
        User user = (User)HttpContext.Items["User"]!;
        var mapper = new UserMapper();
        return (await _userService.GetUserBalootSettings(user.Id))
        .Handle<UserBalootSettings, IActionResult>(
            (settings) =>
            {
                return Ok(new
                {
                    data = new { balootSettings = mapper.UserBalootSettingsToDto(settings) },
                    message = "Settings fetched successfully."
                });
            }
            , BadRequest);
    }
    #endregion

}
