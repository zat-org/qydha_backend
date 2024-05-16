namespace Qydha.Domain.Services.Implementation;

public class UserService(IUserRepo userRepo, IMessageService messageService, ILogger<UserService> logger, IFileService fileService, IMailingService mailingService, OtpManager otpManager, IUpdatePhoneOTPRequestRepo updatePhoneOTPRequestRepo, IUpdateEmailRequestRepo updateEmailRequestRepo, IOptions<AvatarSettings> avatarOptions, IPhoneAuthenticationRequestRepo phoneAuthenticationRequestRepo, IUserGeneralSettingsRepo userGeneralSettingsRepo, IUserHandSettingsRepo handSettingsRepo, IUserBalootSettingsRepo balootSettingsRepo) : IUserService
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
    private readonly IUserHandSettingsRepo _userHandSettingsRepo = handSettingsRepo;
    private readonly IUserBalootSettingsRepo _userBalootSettingsRepo = balootSettingsRepo;

    private readonly AvatarSettings _avatarSettings = avatarOptions.Value;
    private readonly ILogger<UserService> _logger = logger;
    #endregion

    #region Get User 
    public async Task<Result<User>> GetUserById(Guid userId) => await _userRepo.GetByIdAsync(userId);
    public async Task<Result<User>> GetUserWithSettingsByIdAsync(Guid userId) =>
        await _userRepo.GetUserWithSettingsByIdAsync(userId);

    public async Task<Result> IsUserNameAvailable(string username) => await _userRepo.IsUsernameAvailable(username);

    public async Task<Result<IEnumerable<User>>> GetAllRegularUsers() =>
      await _userRepo.GetAllRegularUsers();
    #endregion

    #region Update User

    public async Task<Result<User>> UpdateUser(User user) =>
                await _userRepo.UpdateAsync(user);

    public async Task<Result<User>> UpdateUserPassword(Guid userId, string oldPassword, string newPassword)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, oldPassword);
        return checkingRes
            .OnSuccessAsync(async (user) =>
                (await _userRepo.UpdateUserPassword(userId, PasswordHashingManager.HashPassword(newPassword))).ToResult(user));
    }

    public async Task<Result<User>> UpdateUserPassword(Guid userId, Guid phoneAuthReqId, string newPassword)
    {
        return (await _phoneAuthenticationRequestRepo.GetByIdAsync(phoneAuthReqId))
        .OnSuccess((req) => req.IsValidToUpdatePasswordUsingIt(userId))
        .OnSuccessAsync(async () => await _userRepo.UpdateUserPassword(userId, PasswordHashingManager.HashPassword(newPassword)))
        .OnSuccessAsync(async () => await _userRepo.GetByIdAsync(userId));
    }

    public async Task<Result> UpdateFCMToken(Guid userId, string token) =>
        await _userRepo.UpdateUserFCMToken(userId, token);
    public async Task<Result<User>> UpdateUserUsername(Guid userId, string password, string newUsername)
    {
        Result<User> checkingRes = await _userRepo.CheckUserCredentials(userId, password);
        return checkingRes
            .OnSuccessAsync(async (user) => (await _userRepo.IsUsernameAvailable(newUsername, userId)).ToResult(user))
            .OnSuccessAsync(async (user) => (await _userRepo.UpdateUserUsername(userId, newUsername)).ToResult(user))
            .OnSuccess(user =>
            {
                user.Username = newUsername;
                return Result.Ok(user);
            });
    }
    public async Task<Result<UpdatePhoneRequest>> UpdateUserPhone(Guid userId, string password, string newPhone)
    {
        return (await _userRepo.CheckUserCredentials(userId, password))
            .OnSuccessAsync(async (user) => (await _userRepo.IsPhoneAvailable(newPhone)).ToResult(user))
            .OnSuccessAsync(async (user) =>
            {
                var otp = _otpManager.GenerateOTP();
                return (await _messageService.SendOtpAsync(newPhone, user.Username!, otp)).ToResult((sender) => (otp, sender));
            })
            .OnSuccessAsync(async (tuple) => await _updatePhoneOTPRequestRepo.AddAsync(new(newPhone, tuple.otp, userId, tuple.sender)));
    }
    public async Task<Result<User>> ConfirmPhoneUpdate(Guid userId, string code, Guid requestId)
    {
        Result<UpdatePhoneRequest> getUPhoneRes = await _updatePhoneOTPRequestRepo.GetByIdAsync(requestId);
        return getUPhoneRes
        .OnSuccess(otpReq => otpReq.IsRequestValidToUse(userId, code, _otpManager).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _updatePhoneOTPRequestRepo.MarkRequestAsUsed(requestId)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.IsPhoneAvailable(otpReq.Phone)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.UpdateUserPhone(otpReq.UserId, otpReq.Phone)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => await _userRepo.GetByIdAsync(otpReq.UserId));
    }
    public async Task<Result<UpdateEmailRequest>> UpdateUserEmail(Guid userId, string password, string newEmail)
    {
        return (await _userRepo.CheckUserCredentials(userId, password))
            .OnSuccessAsync(async (user) => (await _userRepo.IsEmailAvailable(newEmail, user.Id)).ToResult(user))
            .OnSuccessAsync(async (user) =>
            {
                string otp = _otpManager.GenerateOTP();
                return await _mailingService.SendOtpToEmailAsync(newEmail, otp, user);
            })
            .OnSuccessAsync(async (tuple) => await _updateEmailRequestRepo.AddAsync(new(tuple.requestId, newEmail, tuple.otp, userId, tuple.sender)));
    }
    public async Task<Result<User>> ConfirmEmailUpdate(Guid userId, string code, Guid requestId)
    {
        return (await _updateEmailRequestRepo.GetByIdAsync(requestId))
        .OnSuccess((otpReq) => otpReq.IsRequestValidToUse(userId, code, _otpManager).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _updateEmailRequestRepo.MarkRequestAsUsed(requestId)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.IsEmailAvailable(otpReq.Email, otpReq.UserId)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.UpdateUserEmail(otpReq.UserId, otpReq.Email)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => await _userRepo.GetByIdAsync(otpReq.UserId));
    }

    public async Task<Result<User>> UploadUserPhoto(Guid userId, IFormFile file)
    {
        return (await _userRepo.GetByIdAsync(userId))
        .OnSuccessAsync(async (user) =>
        {
            if (user.AvatarPath is not null)
            {
                await _fileService.DeleteFile(user.AvatarPath);
                user.AvatarPath = null;
                user.AvatarUrl = null;
            }
            return await _fileService.UploadFile(_avatarSettings.FolderPath, file);
        })
        .OnSuccessAsync(async (fileData) => await _userRepo.UpdateUserAvatarData(userId, fileData.Path, fileData.Url))
        .OnSuccessAsync(async () => await _userRepo.GetByIdAsync(userId));
    }


    #endregion

    #region Delete User
    public async Task<Result<User>> DeleteUser(Guid userId, string password)
    {
        // TODO log delete user action 
        Result<User> checkUserCredentials = await _userRepo.CheckUserCredentials(userId, password);
        return checkUserCredentials
        .OnSuccessAsync(async (user) => (await _userRepo.DeleteAsync(user.Id)).ToResult(user))
        .OnSuccessAsync(async (user) =>
        {
            if (user.AvatarPath is not null)
                await _fileService.DeleteFile(user.AvatarPath);
            return Result.Ok(user);
        });
    }


    #endregion

    #region user  settings
    public async Task<Result<UserGeneralSettings>> GetUserGeneralSettings(Guid userId) =>
        await _userGeneralSettingsRepo.GetByUserIdAsync(userId);

    public async Task<Result<UserHandSettings>> GetUserHandSettings(Guid userId) =>
        await _userHandSettingsRepo.GetByUserIdAsync(userId);

    public async Task<Result<UserBalootSettings>> GetUserBalootSettings(Guid userId) =>
        await _userBalootSettingsRepo.GetByUserIdAsync(userId);

    public async Task<Result<UserGeneralSettings>> UpdateUserGeneralSettings(UserGeneralSettings settings) =>
           await _userGeneralSettingsRepo.UpdateByUserIdAsync(settings);
    public async Task<Result<UserHandSettings>> UpdateUserHandSettings(UserHandSettings settings) =>
            await _userHandSettingsRepo.UpdateByUserIdAsync(settings);
    public async Task<Result<UserBalootSettings>> UpdateUserBalootSettings(UserBalootSettings settings) =>
            await _userBalootSettingsRepo.UpdateByUserIdAsync(settings);
    #endregion
}