namespace Qydha.Domain.Services.Implementation;

public class AuthService(TokenManager tokenManager, INotificationService notificationService, IUserRepo userRepo, OtpManager otpManager, IRegistrationOTPRequestRepo registrationOTPRequestRepo, IMessageService messageService, IPhoneAuthenticationRequestRepo phoneAuthenticationRequestRepo, IUserGeneralSettingsRepo userGeneralSettingsRepo) : IAuthService
{
    #region  injections
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IRegistrationOTPRequestRepo _registrationOTPRequestRepo = registrationOTPRequestRepo;
    private readonly IPhoneAuthenticationRequestRepo _phoneAuthenticationRequestRepo = phoneAuthenticationRequestRepo;
    private readonly IUserGeneralSettingsRepo _userGeneralSettingsRepo = userGeneralSettingsRepo;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IMessageService _messageService = messageService;
    private readonly OtpManager _otpManager = otpManager;
    private readonly TokenManager _tokenManager = tokenManager;
    #endregion


    public async Task<Result<Tuple<User, string>>> LoginAsAnonymousAsync()
    {
        User user = User.CreateAnonymousUser();
        return (await _userRepo.AddAsync<Guid>(user))
            .OnSuccess((user) =>
            {
                string token = _tokenManager.Generate(user.GetClaims());
                return Result.Ok<Tuple<User, string>>(new(user, token));
            });
    }

    public async Task<Result<Tuple<User, string>>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId)
    {

        return (await _registrationOTPRequestRepo.GetByUniquePropAsync(nameof(RegistrationOTPRequest.Id), requestId))
        .OnSuccess<RegistrationOTPRequest>(otp_request =>
        {
            if (otp_request.OTP != otpCode)
                return Result.Fail<RegistrationOTPRequest>(new()
                {
                    Code = ErrorType.IncorrectOTP,
                    Message = "InCorrect OTP."
                });

            if (!_otpManager.IsOtpValid(otp_request.CreatedAt))
                return Result.Fail<RegistrationOTPRequest>(new()
                {
                    Code = ErrorType.OTPExceededTimeLimit,
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
            if (!string.IsNullOrEmpty(fcm_token))
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
            RegistrationOTPRequest registrationOTP = new(username, phone, passwordHash, otp, userId, fcmToken);
            return await _registrationOTPRequestRepo.AddAsync<Guid>(registrationOTP);
        });
    }

    public async Task<Result> Logout(Guid userId) =>

        await _userRepo.UpdateUserFCMToken(userId, string.Empty);

    private async Task<Result<User>> SaveUserFromRegistrationOTPRequest(RegistrationOTPRequest otpRequest)
    {
        Result<User> saveUserRes;
        if (otpRequest.UserId.HasValue)
            saveUserRes = (await _userRepo.GetByIdAsync(otpRequest.UserId.Value))
                                .OnSuccess<User>((user) => Result.Ok(user.UpdateUserFromRegisterRequest(otpRequest)))
                                .OnSuccessAsync<User>(_userRepo.PutByIdAsync);
        else
            saveUserRes = await _userRepo.AddAsync<Guid>(User.CreateUserFromRegisterRequest(otpRequest));

        return saveUserRes.OnSuccessAsync<User>(async (user) =>
            {
                await _notificationService.SendToUserPreDefinedNotification(user, SystemDefaultNotifications.Register);
                return Result.Ok(user);
            });
    }

    public async Task<Result<PhoneAuthenticationRequest>> RequestPhoneAuthentication(string phone)
    {
        return (await _userRepo.GetByPhoneAsync(phone))
       .OnSuccessAsync(async (user) =>
       {
           string otp = _otpManager.GenerateOTP();
           return (await _messageService.SendAsync(user.Phone!, otp)).MapTo(new Tuple<User, string>(user, otp));
       })
       .OnSuccessAsync(async (tuple) =>
           await _phoneAuthenticationRequestRepo.AddAsync<Guid>(new PhoneAuthenticationRequest(tuple.Item1.Phone!, tuple.Item2))
       );
    }

    public async Task<Result<Tuple<User, string>>> ConfirmPhoneAuthentication(Guid requestId, string otpCode, string? fcmToken)
    {
        return (await _phoneAuthenticationRequestRepo.GetByUniquePropAsync(nameof(PhoneAuthenticationRequest.Id), requestId))
        .OnSuccess<PhoneAuthenticationRequest>((otp_request) =>
        {
            if (otp_request.Otp != otpCode)
                return Result.Fail<PhoneAuthenticationRequest>(new()
                {
                    Code = ErrorType.IncorrectOTP,
                    Message = "InCorrect OTP."
                });

            if (!_otpManager.IsOtpValid(otp_request.CreatedAt))
                return Result.Fail<PhoneAuthenticationRequest>(new()
                {
                    Code = ErrorType.OTPExceededTimeLimit,
                    Message = "OTP Exceed Time Limit"
                });
            return Result.Ok(otp_request);
        })
        .OnSuccessAsync(async (request) => await _userRepo.GetByPhoneAsync(request.Phone))
        .OnSuccessAsync<User>(async (user) =>
        {
            if (!string.IsNullOrWhiteSpace(fcmToken))
                return (await _userRepo.UpdateUserFCMToken(user.Id, fcmToken)).MapTo(user);
            return Result.Ok(user);
        })
        .OnSuccess((user) => Result.Ok(new Tuple<User, string>(user, _tokenManager.Generate(user.GetClaims()))));
    }
}