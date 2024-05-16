
namespace Qydha.Domain.Services.Implementation;

public class AuthService(TokenManager tokenManager, IMediator mediator, IUserRepo userRepo, OtpManager otpManager, IRegistrationOTPRequestRepo registrationOTPRequestRepo, IMessageService messageService, IPhoneAuthenticationRequestRepo phoneAuthenticationRequestRepo, ILoginWithQydhaOtpSenderService loginWithQydhaOtpSenderService, ILoginWithQydhaRequestRepo loginWithQydhaRequestRepo) : IAuthService
{
    #region  injections
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMediator _mediator = mediator;
    private readonly IRegistrationOTPRequestRepo _registrationOTPRequestRepo = registrationOTPRequestRepo;
    private readonly IPhoneAuthenticationRequestRepo _phoneAuthenticationRequestRepo = phoneAuthenticationRequestRepo;
    private readonly IMessageService _messageService = messageService;
    private readonly ILoginWithQydhaOtpSenderService _loginWithQydhaOtpSenderService = loginWithQydhaOtpSenderService;
    private readonly ILoginWithQydhaRequestRepo _loginWithQydhaRequestRepo = loginWithQydhaRequestRepo;
    private readonly OtpManager _otpManager = otpManager;
    private readonly TokenManager _tokenManager = tokenManager;
    #endregion

    public async Task<Result<(User user, string jwtToken)>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId)
    {

        return (await _registrationOTPRequestRepo.GetByIdAsync(requestId))
        .OnSuccess(otpReq => otpReq.IsRequestValidToUse(_otpManager, otpCode).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.IsUsernameAvailable(otpReq.Username)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.IsPhoneAvailable(otpReq.Phone)).ToResult(otpReq))
        .OnSuccessAsync(SaveUserFromRegistrationOTPRequest)
        .OnSuccessAsync(async (user) => (await _registrationOTPRequestRepo.MarkRequestAsUsed(requestId, user.Id)).ToResult(user))
        .OnSuccess((user) =>
        {
            string jwtToken = _tokenManager.Generate(user.GetClaims());
            return Result.Ok((user, jwtToken));
        });
    }

    public async Task<Result<(User user, string jwtToken)>> Login(string username, string password, string? fcm_token)
    {
        return (await _userRepo.CheckUserCredentials(username, password))
        .OnSuccessAsync(async (user) => (await _userRepo.UpdateUserLastLoginToNow(user.Id)).ToResult(user))
        .OnSuccessAsync(async (user) =>
        {
            if (!string.IsNullOrEmpty(fcm_token))
                return (await _userRepo.UpdateUserFCMToken(user.Id, fcm_token)).ToResult(user);
            return Result.Ok(user);
        })
        .OnSuccess((user) => Result.Ok((user, _tokenManager.Generate(user.GetClaims()))));
    }

    public async Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken)
    {
        Result result = await _userRepo.IsUsernameAvailable(username);
        return result
        .OnSuccessAsync(async () => await _userRepo.IsPhoneAvailable(phone))
        .OnSuccessAsync(async () =>
        {
            var otp = _otpManager.GenerateOTP();
            return (await _messageService.SendOtpAsync(phone, username, otp)).ToResult((sender) => (sender, otp));
        })
        .OnSuccessAsync(async (tuple) =>
        {
            string passwordHash = PasswordHashingManager.HashPassword(password);
            RegistrationOTPRequest registrationOTP = new(username, phone, passwordHash, tuple.otp, fcmToken, tuple.sender);
            return await _registrationOTPRequestRepo.AddAsync(registrationOTP);
        });
    }

    public async Task<Result> Logout(Guid userId) =>
        await _userRepo.UpdateUserFCMToken(userId, string.Empty);

    private async Task<Result<User>> SaveUserFromRegistrationOTPRequest(RegistrationOTPRequest otpRequest)
    {
        Result<User> saveUserRes = await _userRepo.AddAsync(User.CreateUserFromRegisterRequest(otpRequest));
        return saveUserRes.OnSuccessAsync(async (user) =>
        {
            await _mediator.Publish(new UserRegistrationNotification(saveUserRes.Value));
            return Result.Ok(user);
        });
    }

    public async Task<Result<PhoneAuthenticationRequest>> RequestPhoneAuthentication(string phone)
    {
        return (await _userRepo.GetByPhoneAsync(phone))
       .OnSuccessAsync(async (user) =>
       {
           string otp = _otpManager.GenerateOTP();
           return (await _messageService.SendOtpAsync(user.Phone!, user.Username!, otp)).ToResult((sender) => (user, otp, sender));
       })
       .OnSuccessAsync(async (tuple) =>
           await _phoneAuthenticationRequestRepo.AddAsync(new PhoneAuthenticationRequest(tuple.user.Id, tuple.otp, tuple.sender)));
    }

    public async Task<Result<(User user, string jwtToken)>> ConfirmPhoneAuthentication(Guid requestId, string otpCode, string? fcmToken)
    {
        return (await _phoneAuthenticationRequestRepo.GetByIdAsync(requestId))
        .OnSuccess((otpReq) => otpReq.IsValidToConfirmPhoneAuthUsingIt(_otpManager, otpCode).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _phoneAuthenticationRequestRepo.MarkRequestAsUsed(requestId)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => await _userRepo.GetByIdAsync(otpReq.UserId))
        .OnSuccessAsync(async (user) =>
        {
            if (!string.IsNullOrWhiteSpace(fcmToken))
                return (await _userRepo.UpdateUserFCMToken(user.Id, fcmToken)).ToResult(user);
            return Result.Ok(user);
        })
        .OnSuccess((user) => Result.Ok((user, _tokenManager.Generate(user.GetClaims()))));
    }

    public async Task<Result<LoginWithQydhaRequest>> SendOtpToLoginWithQydha(string username, string serviceConsumerName)
    {
        return (await _userRepo.GetByUsernameAsync(username))
        .OnSuccessAsync(async (user) =>
        {
            string otp = _otpManager.GenerateOTP();
            return (await _loginWithQydhaOtpSenderService.SendOtpAsync(user, otp, serviceConsumerName))
                .ToResult((_) => (user, otp));
        })
        .OnSuccessAsync(async (tuple) => await _loginWithQydhaRequestRepo.AddAsync(new LoginWithQydhaRequest(tuple.user.Id, tuple.otp)));
    }

    public async Task<Result<(User user, string jwtToken)>> ConfirmLoginWithQydha(Guid requestId, string otpCode)
    {
        return (await _loginWithQydhaRequestRepo.GetByIdAsync(requestId))
        .OnSuccess((otpReq) => otpReq.IsRequestValidToUse(_otpManager, otpCode).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _loginWithQydhaRequestRepo.MarkRequestAsUsed(requestId)).ToResult(otpReq))
        .OnSuccessAsync(async (request) => await _userRepo.GetByIdAsync(request.UserId))
        .OnSuccess((user) => Result.Ok((user, _tokenManager.Generate(user.GetClaims()))));
    }

}