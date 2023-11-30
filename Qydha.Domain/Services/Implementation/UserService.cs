namespace Qydha.Domain.Services.Implementation;

public class UserService : IUserService
{
    #region  injections
    private readonly OtpManager _otpManager;
    private readonly IMessageService _messageService;
    private readonly IFileService _fileService;
    private readonly IMailingService _mailingService;
    private readonly IUserRepo _userRepo;
    private readonly IUpdateEmailRequestRepo _updateEmailRequestRepo;
    private readonly IUpdatePhoneOTPRequestRepo _updatePhoneOTPRequestRepo;
    private readonly AvatarSettings _avatarSettings;
    #endregion

    public UserService(IUserRepo userRepo, IMessageService messageService, IFileService fileService, IMailingService mailingService, OtpManager otpManager, IUpdatePhoneOTPRequestRepo updatePhoneOTPRequestRepo, IUpdateEmailRequestRepo updateEmailRequestRepo, IOptions<AvatarSettings> avatarOptions)
    {
        _userRepo = userRepo;
        _updatePhoneOTPRequestRepo = updatePhoneOTPRequestRepo;
        _messageService = messageService;
        _otpManager = otpManager;
        _mailingService = mailingService;
        _updateEmailRequestRepo = updateEmailRequestRepo;
        _fileService = fileService;
        _avatarSettings = avatarOptions.Value;
    }

    #region Get User 
    public async Task<Result<User>> GetUserById(Guid userId) => await _userRepo.GetByIdAsync(userId);

    #endregion

    #region Update User

    public async Task<Result<User>> UpdateUser(User user) =>
                await _userRepo.PutByIdAsync(user);

    public async Task<Result<User>> UpdateUserPassword(Guid userId, string oldPassword, string newPassword)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, oldPassword);
        return checkingRes
            .OnSuccessAsync<User>(async (user) =>
                (await _userRepo.PatchById(userId, "password_hash", BCrypt.Net.BCrypt.HashPassword(newPassword)))
                .MapTo(user));
    }
    public async Task<Result<User>> UpdateUserUsername(Guid userId, string password, string newUsername)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, password);
        return checkingRes
            .OnSuccessAsync<User>(async (user) => (await _userRepo.IsUsernameAvailable(newUsername)).MapTo(user))
            .OnSuccessAsync<User>(async (user) => (await _userRepo.PatchById(userId, "username", newUsername)).MapTo(user))
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
            .OnSuccessAsync(async (otp) => await _updatePhoneOTPRequestRepo.AddAsync(new(newPhone, otp, userId)));
    }
    public async Task<Result<User>> ConfirmPhoneUpdate(Guid userId, string code, Guid requestId)
    {
        Result<UpdatePhoneRequest> getUPhoneRes = await _updatePhoneOTPRequestRepo.GetByIdAsync(requestId);
        return getUPhoneRes.OnSuccess<UpdatePhoneRequest>((otp_request) =>
            {
                if (!_otpManager.IsOtpValid(otp_request.Created_On))
                    return Result.Fail<UpdatePhoneRequest>(new()
                    {
                        Code = ErrorCodes.OTPExceededTimeLimit,
                        Message = "OTP Exceed Time Limit"
                    });

                if (otp_request.OTP != code || otp_request.User_Id != userId)
                    return Result.Fail<UpdatePhoneRequest>(new()
                    {
                        Code = ErrorCodes.InvalidOTP,
                        Message = "Invalid OTP."
                    });

                return Result.Ok(otp_request);
            })
            .OnSuccessAsync<UpdatePhoneRequest>(async (otp_request) => (await _userRepo.IsPhoneAvailable(otp_request.Phone)).MapTo(otp_request))
            .OnSuccessAsync<UpdatePhoneRequest>(async (otp_request) => (await _userRepo.PatchById(otp_request.User_Id, "phone", otp_request.Phone)).MapTo(otp_request))
            .OnSuccessAsync(async (otp_request) => await _userRepo.GetByIdAsync(otp_request.User_Id));
    }
    public async Task<Result<UpdateEmailRequest>> UpdateUserEmail(Guid userId, string password, string newEmail)
    {
        Result checkingRes = await _userRepo.CheckUserCredentials(userId, password);
        return checkingRes
            .OnSuccessAsync(async () => await _userRepo.IsEmailAvailable(newEmail))
            .OnSuccessAsync(async () =>
            {
                string otp = _otpManager.GenerateOTP();
                Guid requestId = Guid.NewGuid();
                var emailSubject = "تأكيد البريد الالكتروني لحساب تطبيق قيدها";
                var emailBody = _mailingService.GenerateConfirmEmailBody(otp, requestId.ToString());
                return (await _mailingService.SendEmailAsync(newEmail, emailSubject, emailBody))
                        .MapTo(new Tuple<string, Guid>(otp, requestId));
            })
            .OnSuccessAsync(async (tuple) => await _updateEmailRequestRepo.AddAsync(new(tuple.Item2, newEmail, tuple.Item1, userId)));
    }
    public async Task<Result<User>> ConfirmEmailUpdate(Guid userId, string code, Guid requestId)
    {

        Result<UpdateEmailRequest> getUEmailRes = await _updateEmailRequestRepo.GetByIdAsync(requestId);
        return getUEmailRes.OnSuccess<UpdateEmailRequest>((otp_request) =>
        {
            if (!_otpManager.IsOtpValid(otp_request.Created_On))
                return Result.Fail<UpdateEmailRequest>(new()
                {
                    Code = ErrorCodes.OTPExceededTimeLimit,
                    Message = "OTP Exceed Time Limit"
                });

            if (otp_request.OTP != code || otp_request.User_Id != userId)
                return Result.Fail<UpdateEmailRequest>(new()
                {
                    Code = ErrorCodes.InvalidOTP,
                    Message = "Invalid OTP."
                });

            return Result.Ok(otp_request);
        })
        .OnSuccessAsync<UpdateEmailRequest>(async (otp_request) => (await _userRepo.IsEmailAvailable(otp_request.Email)).MapTo(otp_request))
        .OnSuccessAsync<UpdateEmailRequest>(async (otp_request) =>
            (await _userRepo.PatchById(otp_request.User_Id,
                new() { { "email", otp_request.Email }, { "is_email_confirmed", true } }))
            .MapTo(otp_request))
        .OnSuccessAsync(async (otp_request) => await _userRepo.GetByIdAsync(otp_request.User_Id));
    }

    public async Task<Result<User>> UploadUserPhoto(Guid userId, IFormFile file)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccessAsync(async (user) =>
        {
            if (user.Avatar_Path is not null)
            {
                (await _fileService.DeleteFile(user.Avatar_Path))
                .OnSuccess(() =>
                {
                    user.Avatar_Path = null;
                    user.Avatar_Url = null;
                })
                .OnFailure(() =>
                {
                    //! TODO :: Handle Delete File Error
                });
            }
            return await _fileService.UploadFile(_avatarSettings.FolderPath, file);
        })
        .OnSuccessAsync<FileData>(async (fileData) =>
            (await _userRepo.PatchById(userId, new() { { "Avatar_Path", fileData.Path }, { "Avatar_Url", fileData.Url } }))
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
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password_Hash))
                return Result.Fail<User>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        })
        .OnSuccessAsync<User>(async (user) => (await _userRepo.DeleteByIdAsync(user.Id)).MapTo(user))
        .OnSuccessAsync<User>(async (user) =>
        {
            if (user.Avatar_Path is not null)
                (await _fileService.DeleteFile(user.Avatar_Path))
                .OnFailure(() =>
                {
                    //! TODO :: Handle Delete File Error 
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
            if (!user.Is_Anonymous)
                return Result.Fail<User>(new()
                {
                    Code = ErrorCodes.InvalidDeleteOnRegularUser,
                    Message = "Can't Delete that Regular User"
                });
            return Result.Ok(user);
        })
        .OnSuccessAsync<User>(async (user) => (await _userRepo.DeleteByIdAsync(user.Id)).MapTo(user));
    }

    public async Task<Result> UpdateFCMToken(Guid userId, string token) =>
        await _userRepo.UpdateUserFCMToken(userId, token);

    #endregion



}