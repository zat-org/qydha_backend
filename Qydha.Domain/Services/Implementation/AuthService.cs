namespace Qydha.Domain.Services.Implementation;

public class AuthService : IAuthService
{
    #region  injections
    private readonly IUserRepo _userRepo;
    private readonly IRegistrationOTPRequestRepo _registrationOTPRequestRepo;
    private readonly INotificationService _notificationService;
    private readonly IMessageService _messageService;
    private readonly OtpManager _otpManager;
    private readonly TokenManager _tokenManager;
    public AuthService(TokenManager tokenManager, INotificationService notificationService, IUserRepo userRepo, OtpManager otpManager, IRegistrationOTPRequestRepo registrationOTPRequestRepo, IMessageService messageService)
    {
        _tokenManager = tokenManager;
        _registrationOTPRequestRepo = registrationOTPRequestRepo;
        _otpManager = otpManager;
        _userRepo = userRepo;
        _messageService = messageService;
        _notificationService = notificationService;

    }
    #endregion


    public async Task<Result<string>> LoginAsAnonymousAsync()
    {
        var user = User.CreateAnonymousUser();
        return (await _userRepo.AddAsync(user))
            .OnSuccess((user) =>
            {
                return Result.Ok(_tokenManager.Generate(user.GetClaims()));
            });
    }

    public async Task<Result<Tuple<User, string>>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId)
    {

        return (await _registrationOTPRequestRepo.GetByIdAsync(requestId))
        .OnSuccess<RegistrationOTPRequest>(otp_request =>
        {
            if (otp_request.OTP != otpCode)
                return Result.Fail<RegistrationOTPRequest>(new()
                {
                    Code = ErrorCodes.InvalidOTP,
                    Message = "Invalid OTP."
                });

            if (!_otpManager.IsOtpValid(otp_request.Created_On))
                return Result.Fail<RegistrationOTPRequest>(new()
                {
                    Code = ErrorCodes.OTPExceededTimeLimit,
                    Message = "OTP Exceed Time Limit"
                });
            return Result.Ok(otp_request);
        })
        .OnSuccessAsync<RegistrationOTPRequest>(async (otp_request) => (await _userRepo.IsUsernameAvailable(otp_request.Username)).MapTo(otp_request))
        .OnSuccessAsync<RegistrationOTPRequest>(async (otp_request) => (await _userRepo.IsPhoneAvailable(otp_request.Phone)).MapTo(otp_request))
        .OnSuccessAsync(SaveUserFromRegistrationOTPRequest)
        .OnSuccess((user) =>
        {
            var jwtToken = _tokenManager.Generate(user.GetClaims());
            return Result.Ok(new Tuple<User, string>(user, jwtToken));
        });
    }

    public async Task<Result<Tuple<User, string>>> Login(string username, string password, string? fcm_token)
    {
        return (await _userRepo.CheckUserCredentials(username, password))
        .OnSuccessAsync<User>(async (user) => (await _userRepo.UpdateUserLastLoginToNow(user.Id)).MapTo(user))
        .OnSuccessAsync<User>(async (user) =>
        {
            if (fcm_token is not null)
                return (await _userRepo.UpdateUserFCMToken(user.Id, fcm_token)).MapTo(user);
            return Result.Ok(user);
        })
        .OnSuccess((user) =>
        {
            var jwtToken = _tokenManager.Generate(user.GetClaims());
            return Result.Ok(new Tuple<User, string>(user, jwtToken));
        });
    }

    public async Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken, Guid? userId)
    {
        Result result;

        if (userId is not null)
            result = (await _userRepo.GetByIdAsync(userId.Value))
            .OnSuccessAsync(async () => await _userRepo.IsUsernameAvailable(username));
        else
            result = await _userRepo.IsUsernameAvailable(username);

        return result
        .OnSuccessAsync(async () => await _userRepo.IsPhoneAvailable(phone))
        .OnSuccessAsync(async () =>
        {
            var otp = _otpManager.GenerateOTP();
            return (await _messageService.SendAsync(phone, otp)).MapTo(otp);
        })
        .OnSuccessAsync(async (otp) =>
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return await _registrationOTPRequestRepo.AddAsync(new(username, phone, passwordHash, otp, userId, fcmToken));
        });
    }

    public async Task<Result> Logout(Guid userId) =>

        await _userRepo.UpdateUserFCMToken(userId, string.Empty);

    private async Task<Result<User>> SaveUserFromRegistrationOTPRequest(RegistrationOTPRequest otpRequest)
    {
        Result<User> saveUserRes;
        if (otpRequest.User_Id.HasValue)
            saveUserRes = (await _userRepo.GetByIdAsync(otpRequest.User_Id.Value))
                                .OnSuccess<User>((user) => Result.Ok(user.UpdateUserFromRegisterRequest(otpRequest)))
                                .OnSuccessAsync<User>(async (user) => await _userRepo.PutByIdAsync(user));
        else
            saveUserRes = await _userRepo.AddAsync(User.CreateUserFromRegisterRequest(otpRequest));

        return saveUserRes.OnSuccessAsync<User>(async (user) =>
            {
                await _notificationService.SendToUser(user, Notification.CreateRegisterNotification(user));
                return Result.Ok(user);
            });
    }
}