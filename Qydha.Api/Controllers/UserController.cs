

namespace Qydha.API.Controllers;

[ApiController]
[Route("users/")]
public class UserController(IUserService userService, INotificationService notificationService) : ControllerBase
{
    #region injections and ctor
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;

    #endregion


    #region Get user 
    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParameters paginationParameters, [FromQuery] UsersFilterParameters filterParameters)
    {
        return (await _userService.GetAllRegularUsers(paginationParameters, filterParameters))
            .Resolve((users) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { users = mapper.PageListToUserPageDto(users) },
                    message = "users fetched successfully."
                });
            }, HttpContext.TraceIdentifier);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> GetUserByIdForDashboard([FromRoute] Guid id)
    {
        return (await _userService.GetByIdForDashboardAsync(id))
            .Resolve((user) =>
            {
                var userMapper = new UserMapper();
                var influencerCodeMapper = new InfluencerCodeMapper();
                var promoCodeMapper = new UserPromoCodeMapper();
                var purchaseMapper = new PurchasesMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = userMapper.UserToUserDto(user),
                        PromoCodes = user.UserPromoCodes.Select(promoCodeMapper.PromoCodeToGetUsedPromoCodeDto),
                        Purchases = user.Purchases.Select(purchaseMapper.PurchaseToGetUserPurchaseDto),
                        InfluencerCodes = user.InfluencerCodes.Select(influencerCodeMapper.InfluencerCodeUserLinkToInfluencerCodeUsedByUser)
                    },
                    message = "user fetched successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [HttpGet("me/")]
    [Authorize(Roles = RoleConstants.User)]
    public IActionResult GetUser()
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(_userService.GetUserWithSettingsByIdAsync)
            .Resolve(
                (userData) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        data = new
                        {
                            user = mapper.UserToUserDto(userData),
                            generalSettings = userData.UserGeneralSettings is null ? null : mapper.UserGeneralSettingsToDto(userData.UserGeneralSettings),
                            handSettings = userData.UserHandSettings is null ? null : mapper.UserHandSettingsToDto(userData.UserHandSettings),
                            balootSettings = userData.UserBalootSettings is null ? null : mapper.UserBalootSettingsToDto(userData.UserBalootSettings)
                        },
                        message = "User fetched successfully."
                    });
                },
                HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.ServiceAccountPermission)]
    [Permission(ServiceAccountPermission.CheckUserNameAvailable)]
    [HttpGet("is-username-available")]
    public async Task<IActionResult> IsUserNameAvailable([FromBody] string username)
    {
        return (await _userService.IsUserNameAvailable(username))
        .Resolve(
            () =>
            {
                return Ok(new
                {
                    data = new { IsAvailable = true },
                    message = "usernames is available."
                });
            }, HttpContext.TraceIdentifier);
    }

    #endregion

    #region update user
    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-password/")]
    public IActionResult UpdateAuthorizedUserPassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) =>
                await _userService.UpdateUserPassword(id, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
            .Resolve((user) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        data = new { user = mapper.UserToUserDto(user) },
                        message = "User updated successfully."
                    });
                }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-password-from-phone-authentication/")]
    public IActionResult UpdatePhoneAuthorizedUserPassword([FromBody] UpdatePasswordFromPhoneAuthentication dto)
    {

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.UpdateUserPassword(id, dto.RequestId, dto.NewPassword))
        .Resolve((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-username/")]
    public IActionResult UpdateAuthorizedUsername([FromBody] ChangeUsernameDto changeUsernameDto)
    {

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.UpdateUserUsername(id, changeUsernameDto.Password, changeUsernameDto.NewUsername))
        .Resolve((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-phone/")]
    public IActionResult UpdateAuthorizedPhone([FromBody] ChangePhoneDto changePhoneDto)
    {

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.UpdateUserPhone(id, changePhoneDto.Password, changePhoneDto.NewPhone))
        .Resolve((otp_request) =>
            {
                return Ok(new
                {
                    Data = new { RequestId = otp_request.Id },
                    Message = "Otp sent successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPost("me/confirm-phone-update/")]
    public IActionResult ConfirmPhoneUpdate([FromBody] ConfirmPhoneDto confirmPhoneDto)
    {

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.ConfirmPhoneUpdate(id, confirmPhoneDto.Code, confirmPhoneDto.RequestId))
        .Resolve((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-email")]
    public IActionResult UpdateAuthorizedEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.UpdateUserEmail(id, changeEmailDto.Password, changeEmailDto.NewEmail))
            .Resolve((otp_request) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        Data = new { RequestId = otp_request.Id },
                        Message = "User updated successfully."
                    });
                }, HttpContext.TraceIdentifier);
    }
    [Authorize(Roles = RoleConstants.User)]
    [HttpPost("me/confirm-email-update/")]
    public IActionResult ConfirmEmailUpdate([FromBody] ConfirmEmailDto confirmEmailDto)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.ConfirmEmailUpdate(id, confirmEmailDto.Code, confirmEmailDto.RequestId))
            .Resolve((user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(user) },
                    message = "User updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }
    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-avatar")]
    public IActionResult UpdateUserAvatar([FromForm] UpdateUserAvatarDto dto)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.UploadUserPhoto(id, dto.File))
            .Resolve((user) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        data = new { user = mapper.UserToUserDto(user) },
                        message = "User updated successfully."
                    });
                }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/update-fcm-token")]
    public IActionResult UpdateUsersFCMToken([FromBody] ChangeUserFCMTokenDto changeUserFCMTokenDto)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _userService.UpdateFCMToken(id, changeUserFCMTokenDto.FCMToken))
            .Resolve(() => Ok(new
            {
                data = new { },
                Message = "User fcm token Updated Successfully"
            }), HttpContext.TraceIdentifier);
    }


    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/")]
    public IActionResult UpdateUserData([FromBody] JsonPatchDocument<UpdateUserDto> updateUserDtoPatch)
    {
        var mapper = new UserMapper();
        return HttpContext.User.GetUserIdentifier()
        .OnSuccessAsync((userId) => _userService.GetUserById(userId, withTracking: true))
        .OnSuccess((user) =>
        {
            var dto = mapper.UserToUpdateUserDto(user);
            return updateUserDtoPatch.ApplyToAsResult(dto)
            .OnSuccess((dtoWithChanges) =>
            {
                var validator = new UpdateUserDtoValidator();
                return validator.ValidateAsResult(dtoWithChanges);
            })
            .OnSuccess((dtoWithChanges) =>
            {
                mapper.ApplyUpdateUserDtoToUser(dtoWithChanges, user);
                return Result.Ok(user);
            });
        })
        .OnSuccessAsync(_userService.UpdateUser)
        .Resolve((user) =>
        {
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user) },
                message = "User updated Successfully"
            });
        }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.SuperAdmin)]
    [HttpPatch("{id}/change-user-roles")]
    public async Task<IActionResult> ChangeUsersRoles([FromRoute] Guid id, [FromBody] ChangeUserRolesDto dto)
    {
        var mapper = new UserMapper();
        return (await _userService.ChangeUserRoles(id, dto.Roles))
        .Resolve((user) =>
        {
            return Ok(new
            {
                data = new { user = mapper.UserToUserDto(user) },
                message = "User updated Successfully"
            });
        }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.SuperAdmin)]
    [HttpGet("roles")]
    public IActionResult GetUsersRoles()
    {
        return Ok(new
        {
            data = new
            {
                roles = Enum
                    .GetValues(typeof(UserRoles))
                    .Cast<UserRoles>()
                    .Select(c => c.ToString())
                    .ToList()
            },
            message = "Available User Roles fetched Successfully"
        });
    }

    #endregion

    #region Delete user
    [Authorize(Roles = RoleConstants.User)]
    [HttpDelete("me/")]
    public IActionResult DeleteUser(DeleteUserDto deleteUserDto)
    {
        return HttpContext.User.GetUserIdentifier()
             .OnSuccessAsync(async (id) => await _userService.DeleteUser(id, deleteUserDto.Password))
        .Resolve((user) => Ok(new { data = new { }, message = $"User with username: '{user.Username}' Deleted Successfully." }), HttpContext.TraceIdentifier);
    }


    #endregion

    #region users notifications
    [Authorize(Roles = RoleConstants.User)]
    [HttpGet("me/notifications")]
    public IActionResult GetUserNotifications([FromQuery] PaginationParameters pageParams)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _notificationService.GetByUserId(id, pageParams))
        .Resolve((notifications) =>
            {
                return Ok(new
                {
                    Data = new NotificationMapper().PageListToNotificationPageDto(notifications),
                    Message = "Notifications Fetched successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/notifications/{notificationId}/mark-as-read/")]
    public IActionResult MarkNotificationAsRead([FromRoute] int notificationId)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _notificationService.MarkNotificationAsRead(id, notificationId))
        .Resolve(() => Ok(new { data = new { }, message = "notification marked as read." }), HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/notifications/mark-all-as-read/")]
    public IActionResult MarkAllNotificationAsRead()
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(_notificationService.MarkAllNotificationsOfUserAsRead)
        .Resolve(() => Ok(new { data = new { }, message = "notification marked as read." }), HttpContext.TraceIdentifier);
    }
    [Authorize(Roles = RoleConstants.User)]
    [HttpDelete("me/notifications/{notificationId}")]
    public IActionResult DeleteNotification([FromRoute] int notificationId)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async (id) => await _notificationService.DeleteNotification(id, notificationId))
        .Resolve(() => Ok(new { data = new { }, message = "notification Deleted." }), HttpContext.TraceIdentifier);
    }
    [Authorize(Roles = RoleConstants.User)]
    [HttpDelete("me/notifications/")]
    public IActionResult DeleteAllNotifications()
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(_notificationService.DeleteAll)
        .Resolve(() => Ok(new { data = new { }, message = "All Notifications has been Deleted." }), HttpContext.TraceIdentifier);
    }
    #endregion

    #region user Settings

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/general-settings")]
    public IActionResult UpdateUserGeneralSettings([FromBody] JsonPatchDocument<UserGeneralSettingsDto> generalSettingsDtoPatch)
    {
        var mapper = new UserMapper();

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(userId => _userService.GetUserById(userId, withTracking: true))
            .OnSuccess((user) =>
            {
                var dto = mapper.UserGeneralSettingsToDto(user.UserGeneralSettings);
                return generalSettingsDtoPatch.ApplyToAsResult(dto)
                .OnSuccess((dtoWithChanges) =>
                {
                    mapper.DtoToUserGeneralSettings(dtoWithChanges, user.UserGeneralSettings);
                    return Result.Ok(user);
                });

            })
            .OnSuccessAsync(_userService.UpdateUser)
            .Resolve((user) =>
            {
                return Ok(new
                {
                    data = new
                    {
                        generalSettings = mapper.UserGeneralSettingsToDto(user.UserGeneralSettings)
                    },
                    message = "User's General settings updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/hand-settings")]
    public IActionResult UpdateUserHandSettings([FromBody] JsonPatchDocument<UserHandSettingsDto> handSettingsDtoPatch)
    {
        if (handSettingsDtoPatch is null)
            return BadRequest(new InvalidBodyInputError("لا يوجد بيانات لتحديثها"));

        var mapper = new UserMapper();

        return HttpContext.User.GetUserIdentifier()
        .OnSuccessAsync(userId => _userService.GetUserById(userId, withTracking: true))
        .OnSuccess((user) =>
        {
            var dto = mapper.UserHandSettingsToDto(user.UserHandSettings);
            return handSettingsDtoPatch.ApplyToAsResult(dto)
            .OnSuccess((dtoWithChanges) =>
            {
                var handSettingsValidator = new UserHandSettingsDtoValidator();
                return handSettingsValidator.ValidateAsResult(dtoWithChanges);
            })
            .OnSuccess((dtoWithChanges) =>
            {
                mapper.DtoToUserHandSettings(dtoWithChanges, user.UserHandSettings);
                return Result.Ok(user);
            });
        })
        .OnSuccessAsync(_userService.UpdateUser)
        .Resolve((user) =>
        {
            return Ok(new
            {
                data = new { handSettings = mapper.UserHandSettingsToDto(user.UserHandSettings) },
                message = "User's Hand settings updated successfully."
            });
        }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPatch("me/baloot-settings")]
    public IActionResult UpdateUserBalootSettings([FromBody] JsonPatchDocument<UserBalootSettingsDto> balootSettingsDtoPatch)
    {
        if (balootSettingsDtoPatch is null)
            return BadRequest(new InvalidBodyInputError("لا يوجد بيانات لتحديثها"));
        var mapper = new UserMapper();

        return HttpContext.User.GetUserIdentifier()
        .OnSuccessAsync(userId => _userService.GetUserById(userId, withTracking: true))
        .OnSuccess((user) =>
        {
            var dto = mapper.UserBalootSettingsToDto(user.UserBalootSettings);
            return balootSettingsDtoPatch.ApplyToAsResult(dto)
            .OnSuccess((dtoWithChanges) =>
            {
                mapper.DtoToUserBalootSettings(dtoWithChanges, user.UserBalootSettings);
                return Result.Ok(user);
            });
        })
        .OnSuccessAsync(_userService.UpdateUser)
        .Resolve((user) =>
        {
            return Ok(new
            {
                data = new { balootSettings = mapper.UserBalootSettingsToDto(user.UserBalootSettings) },
                message = "User's baloot settings updated successfully."
            });
        }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpGet("me/general-settings")]
    public IActionResult GetUserGeneralSettings()
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(userId => _userService.GetUserById(userId, withTracking: false))
            .Resolve(
                (user) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        data = new { generalSettings = mapper.UserGeneralSettingsToDto(user.UserGeneralSettings) },
                        message = "Settings fetched successfully."
                    });
                }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpGet("me/hand-settings")]
    public IActionResult GetUserHandSettings()
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(userId => _userService.GetUserById(userId, withTracking: false))
            .Resolve(
                (user) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        data = new { handSettings = mapper.UserHandSettingsToDto(user.UserHandSettings) },
                        message = "Settings fetched successfully."
                    });
                }, HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpGet("me/baloot-settings")]
    public IActionResult GetUserBalootSettings()
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(userId => _userService.GetUserById(userId, withTracking: false))
            .Resolve(
                (user) =>
                {
                    var mapper = new UserMapper();
                    return Ok(new
                    {
                        data = new { balootSettings = mapper.UserBalootSettingsToDto(user.UserBalootSettings) },
                        message = "Settings fetched successfully."
                    });
                }, HttpContext.TraceIdentifier);
    }
    #endregion

}
