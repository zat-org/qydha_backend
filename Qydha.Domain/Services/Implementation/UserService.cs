namespace Qydha.Domain.Services.Implementation;

public class UserService(IUserRepo userRepo, IMessageService messageService, ILogger<UserService> logger, IFileService fileService, IMailingService mailingService, OtpManager otpManager, IUpdatePhoneOTPRequestRepo updatePhoneOTPRequestRepo, IUpdateEmailRequestRepo updateEmailRequestRepo, IOptions<AvatarSettings> avatarOptions, IPhoneAuthenticationRequestRepo phoneAuthenticationRequestRepo, IUserGeneralSettingsRepo userGeneralSettingsRepo) : IUserService
{
    #region  injections
    private readonly OtpManager _otpManager = otpManager;
    private readonly IMessageService _messageService = messageService;
    private readonly IFileService _fileService = fileService;
    private readonly IMailingService _mailingService = mailingService;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IUpdateEmailRequestRepo _updateEmailRequestRepo = updateEmailRequestRepo;
    private readonly IUpdatePhoneOTPRequestRepo _updatePhoneOTPRequestRepo = updatePhoneOTPRequestRepo;
    private readonly IPhoneAuthenticationRequestRepo _phoneAuthenticationRequestRepo = phoneAuthenticationRequestRepo;
    private readonly IUserGeneralSettingsRepo _userGeneralSettingsRepo = userGeneralSettingsRepo;
    private readonly AvatarSettings _avatarSettings = avatarOptions.Value;
    private readonly ILogger<UserService> _logger = logger;
    #endregion

    #region Get User 
    public async Task<Result<User>> GetUserById(Guid userId) => await _userRepo.GetByIdAsync(userId);
    public async Task<Result> IsUserNameAvailable(string username) => await _userRepo.IsUsernameAvailable(username);

    #endregion

    #region Update User

    public async Task<Result<User>> UpdateUser(User user) =>
                await _userRepo.PutByIdAsync(user);

    public async Task<Result<User>> UpdateUserPassword(Guid userId, string oldPassword, string newPassword)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, oldPassword);
        return checkingRes
            .OnSuccessAsync<User>(async (user) =>
                (await _userRepo.UpdateUserPassword(userId, BCrypt.Net.BCrypt.HashPassword(newPassword)))
                .MapTo(user));
    }

    public async Task<Result<User>> UpdateUserPassword(Guid userId, Guid phoneAuthReqId, string newPassword)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccessAsync(
            async (user) =>
                (await _phoneAuthenticationRequestRepo.GetByUniquePropAsync(nameof(PhoneAuthenticationRequest.Id), phoneAuthReqId))
                .MapTo((request) => new Tuple<User, PhoneAuthenticationRequest>(user, request)))
        .OnSuccess((tuple) =>
        {
            if (tuple.Item1.IsAnonymous)
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidActionByAnonymousUser,
                    Message = "Invalid Operation On Anonymous User"
                });

            if (tuple.Item1.Phone != tuple.Item2.Phone)
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidForgetPasswordRequest,
                    Message = "User phone is not the same in the phone login request"
                });

            if (tuple.Item2.CreatedAt.AddDays(1) < DateTime.UtcNow)
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.ForgetPasswordRequestExceedTime,
                    Message = "Forget Password Request Exceed Time of 1 Day"
                });

            return Result.Ok(tuple.Item1);
        })
        .OnSuccessAsync<User>(async (user) =>
                (await _userRepo.UpdateUserPassword(userId, BCrypt.Net.BCrypt.HashPassword(newPassword)))
                .MapTo(user));
    }

    public async Task<Result> UpdateFCMToken(Guid userId, string token) =>
      await _userRepo.UpdateUserFCMToken(userId, token);

    public async Task<Result<User>> UpdateUserUsername(Guid userId, string password, string newUsername)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, password);
        return checkingRes
            .OnSuccessAsync<User>(async (user) => (await _userRepo.IsUsernameAvailable(newUsername, userId)).MapTo(user))
            .OnSuccessAsync<User>(async (user) => (await _userRepo.UpdateUserUsername(userId, newUsername)).MapTo(user))
            .OnSuccess<User>(user =>
            {
                user.Username = newUsername;
                return Result.Ok(user);
            });
    }
    public async Task<Result<UpdatePhoneRequest>> UpdateUserPhone(Guid userId, string password, string newPhone)
    {

        Result checkingRes = await _userRepo.CheckUserCredentials(userId, password);
        return checkingRes
            .OnSuccessAsync(async () => await _userRepo.IsPhoneAvailable(newPhone))
            .OnSuccessAsync(async () =>
            {
                var otp = _otpManager.GenerateOTP();
                return (await _messageService.SendAsync(newPhone, otp)).MapTo(otp);
            })
            .OnSuccessAsync(async (otp) => await _updatePhoneOTPRequestRepo.AddAsync<Guid>(new(newPhone, otp, userId)));
    }
    public async Task<Result<User>> ConfirmPhoneUpdate(Guid userId, string code, Guid requestId)
    {
        Result<UpdatePhoneRequest> getUPhoneRes = await _updatePhoneOTPRequestRepo.GetByUniquePropAsync(nameof(UpdatePhoneRequest.Id), requestId);
        return getUPhoneRes.OnSuccess<UpdatePhoneRequest>((otp_request) =>
            {
                if (!_otpManager.IsOtpValid(otp_request.CreatedAt))
                    return Result.Fail<UpdatePhoneRequest>(new()
                    {
                        Code = ErrorType.OTPExceededTimeLimit,
                        Message = "OTP Exceed Time Limit"
                    });

                if (otp_request.OTP != code || otp_request.UserId != userId)
                    return Result.Fail<UpdatePhoneRequest>(new()
                    {
                        Code = ErrorType.IncorrectOTP,
                        Message = "Incorrect OTP."
                    });

                return Result.Ok(otp_request);
            })
            .OnSuccessAsync<UpdatePhoneRequest>(async (otp_request) => (await _userRepo.IsPhoneAvailable(otp_request.Phone)).MapTo(otp_request))
            .OnSuccessAsync<UpdatePhoneRequest>(async (otp_request) => (await _userRepo.UpdateUserPhone(otp_request.UserId, otp_request.Phone)).MapTo(otp_request))
            .OnSuccessAsync(async (otp_request) => await _userRepo.GetByIdAsync(otp_request.UserId));
    }
    public async Task<Result<UpdateEmailRequest>> UpdateUserEmail(Guid userId, string password, string newEmail)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, password);
        return checkingRes
            .OnSuccessAsync<User>(async (user) => (await _userRepo.IsEmailAvailable(newEmail, user.Id)).MapTo(user))
            .OnSuccessAsync<User, Tuple<string, Guid>>(async (user) =>
            {
                string otp = _otpManager.GenerateOTP();
                Guid requestId = Guid.NewGuid();
                var emailSubject = "تأكيد البريد الالكتروني لحساب تطبيق قيدها";
                var emailBody = await _mailingService.GenerateConfirmEmailBody(otp, requestId.ToString(), user);
                return (await _mailingService.SendEmailAsync(newEmail, emailSubject, emailBody))
                        .MapTo(new Tuple<string, Guid>(otp, requestId));
            })
            .OnSuccessAsync(async (tuple) => await _updateEmailRequestRepo.AddAsync<Guid>(new(tuple.Item2, newEmail, tuple.Item1, userId)));
    }
    public async Task<Result<User>> ConfirmEmailUpdate(Guid userId, string code, Guid requestId)
    {

        Result<UpdateEmailRequest> getUEmailRes = await _updateEmailRequestRepo.GetByUniquePropAsync(nameof(UpdateEmailRequest.Id), requestId);
        return getUEmailRes.OnSuccess<UpdateEmailRequest>((otp_request) =>
        {
            if (!_otpManager.IsOtpValid(otp_request.CreatedAt))
                return Result.Fail<UpdateEmailRequest>(new()
                {
                    Code = ErrorType.OTPExceededTimeLimit,
                    Message = "OTP Exceed Time Limit"
                });

            if (otp_request.OTP != code || otp_request.UserId != userId)
                return Result.Fail<UpdateEmailRequest>(new()
                {
                    Code = ErrorType.IncorrectOTP,
                    Message = "Incorrect OTP."
                });

            return Result.Ok(otp_request);
        })
        .OnSuccessAsync<UpdateEmailRequest>(async (otp_request) => (await _userRepo.IsEmailAvailable(otp_request.Email, otp_request.UserId)).MapTo(otp_request))
        .OnSuccessAsync<UpdateEmailRequest>(async (otp_request) =>
            (await _userRepo.UpdateUserEmail(otp_request.UserId, otp_request.Email))
            .MapTo(otp_request))
        .OnSuccessAsync(async (otp_request) => await _userRepo.GetByIdAsync(otp_request.UserId));
    }

    public async Task<Result<User>> UploadUserPhoto(Guid userId, IFormFile file)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccessAsync(async (user) =>
        {
            if (user.AvatarPath is not null)
            {
                (await _fileService.DeleteFile(user.AvatarPath))
                .OnSuccess(() =>
                {
                    user.AvatarPath = null;
                    user.AvatarUrl = null;
                })
                .OnFailure((result) =>
                {
                    //! TODO :: Handle Delete File Error
                    _logger.LogError(result.Error.ToString());
                });
            }
            return (await _fileService.UploadFile(_avatarSettings.FolderPath, file))
                    .OnFailure((err) =>
                    {
                        //! TODO :: Handle upload File Error
                        _logger.LogError(err.ToString());
                        return new()
                        {
                            Code = ErrorType.FileUploadError,
                            Message = "حدث عطل اثناء حفظ الصورة برجاء المحاولة مرة اخري"
                        };
                    });
        })
        .OnSuccessAsync<FileData>(async (fileData) =>
            (await _userRepo.UpdateUserAvatarData(userId, fileData.Path, fileData.Url))
            .MapTo(fileData))
        .OnSuccessAsync(async () => await _userRepo.GetByIdAsync(userId));
    }


    #endregion

    #region Delete User
    public async Task<Result<User>> DeleteUser(Guid userId, string password)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<User>(new() { Code = ErrorType.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        })
        .OnSuccessAsync<User>(async (user) => (await _userRepo.DeleteByIdAsync(user.Id)).MapTo(user))
        .OnSuccessAsync<User>(async (user) =>
        {
            if (user.AvatarPath is not null)
                (await _fileService.DeleteFile(user.AvatarPath))
                .OnFailure((result) =>
                {
                    //! TODO :: Handle Delete File Error 
                    _logger.LogError(result.Error.ToString());

                });
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> DeleteAnonymousUser(Guid userId)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccess<User>((user) =>
        {
            if (!user.IsAnonymous)
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidActionByRegularUser,
                    Message = "Can't Delete that Regular User"
                });
            return Result.Ok(user);
        })
        .OnSuccessAsync<User>(async (user) => (await _userRepo.DeleteByIdAsync(user.Id)).MapTo(user));
    }
    #endregion

    #region user general settings
    public async Task<Result<UserGeneralSettings>> GetUserGeneralSettings(Guid userId) =>
        await _userGeneralSettingsRepo.GetByUniquePropAsync(nameof(UserGeneralSettings.UserId), userId);
    public async Task<Result<UserGeneralSettings>> UpdateUserGeneralSettings(UserGeneralSettings settings) =>
           await _userGeneralSettingsRepo.PutByIdAsync(settings);



    #endregion
}