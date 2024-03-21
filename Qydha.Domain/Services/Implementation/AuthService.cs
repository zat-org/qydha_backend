
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

    public async Task<Result<Tuple<User, string>>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId)
    {

        return (await _registrationOTPRequestRepo.GetByIdAsync(requestId))
        .OnSuccessAsync<RegistrationOTPRequest>(async otp_request =>
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
            return (await _registrationOTPRequestRepo.MarkRequestAsUsed(requestId)).MapTo(otp_request);
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

    public async Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken)
    {
        Result result = await _userRepo.IsUsernameAvailable(username);
        return result
        .OnSuccessAsync(async () => await _userRepo.IsPhoneAvailable(phone))
        .OnSuccessAsync(async () =>
        {
            var otp = _otpManager.GenerateOTP();
            return (await _messageService.SendOtpAsync(phone, username, otp)).MapTo(otp);
        })
        .OnSuccessAsync(async (otp) =>
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            RegistrationOTPRequest registrationOTP = new(username, phone, passwordHash, otp, fcmToken);
            return await _registrationOTPRequestRepo.AddAsync(registrationOTP);
        });
    }

    public async Task<Result> Logout(Guid userId) =>

        await _userRepo.UpdateUserFCMToken(userId, string.Empty);

    private async Task<Result<User>> SaveUserFromRegistrationOTPRequest(RegistrationOTPRequest otpRequest)
    {
        Result<User> saveUserRes = await _userRepo.AddAsync(User.CreateUserFromRegisterRequest(otpRequest));
        return saveUserRes.OnSuccessAsync<User>(async (user) =>
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
           return (await _messageService.SendOtpAsync(user.Phone!, user.Username!, otp)).MapTo(new Tuple<User, string>(user, otp));
       })
       .OnSuccessAsync(async (tuple) =>
           await _phoneAuthenticationRequestRepo.AddAsync(new PhoneAuthenticationRequest(tuple.Item1.Id, tuple.Item2))
       );
    }

    public async Task<Result<Tuple<User, string>>> ConfirmPhoneAuthentication(Guid requestId, string otpCode, string? fcmToken)
    {
        return (await _phoneAuthenticationRequestRepo.GetByIdAsync(requestId))
        .OnSuccessAsync<PhoneAuthenticationRequest>(async (otp_request) =>
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
            return (await _phoneAuthenticationRequestRepo.MarkRequestAsUsed(requestId)).MapTo(otp_request);
        })
        .OnSuccessAsync(async (request) => await _userRepo.GetByIdAsync(request.UserId))
        .OnSuccessAsync<User>(async (user) =>
        {
            if (!string.IsNullOrWhiteSpace(fcmToken))
                return (await _userRepo.UpdateUserFCMToken(user.Id, fcmToken)).MapTo(user);
            return Result.Ok(user);
        })
        .OnSuccess((user) => Result.Ok(new Tuple<User, string>(user, _tokenManager.Generate(user.GetClaims()))));
    }

    public async Task<Result<LoginWithQydhaRequest>> SendOtpToLoginWithQydha(string username, string serviceConsumerName)
    {
        return (await _userRepo.GetByUsernameAsync(username))
        .OnSuccessAsync(async (user) =>
        {
            // generate otp 
            string otp = _otpManager.GenerateOTP();
            // send otp to the user
            return (await _loginWithQydhaOtpSenderService.SendOtpAsync(user, otp, serviceConsumerName)).MapTo(new Tuple<User, string>(user, otp));
        })
        .OnSuccessAsync(async (tuple) =>
        {
            User user = tuple.Item1;
            string otp = tuple.Item2;
            // save otp  to be validated 
            return await _loginWithQydhaRequestRepo.AddAsync(new LoginWithQydhaRequest(user.Id, otp));
        });
    }

    public async Task<Result<Tuple<User, string>>> ConfirmLoginWithQydha(Guid requestId, string otpCode)
    {
        return (await _loginWithQydhaRequestRepo.GetByIdAsync(requestId))
        .OnSuccessAsync<LoginWithQydhaRequest>(async (otp_request) =>
        {
            if (otp_request.UsedAt is not null)
                return Result.Fail<LoginWithQydhaRequest>(new()
                {
                    Code = ErrorType.OTPAlreadyUsedError,
                    Message = "OTP Already Used."
                });

            if (otp_request.Otp != otpCode)
                return Result.Fail<LoginWithQydhaRequest>(new()
                {
                    Code = ErrorType.IncorrectOTP,
                    Message = "InCorrect OTP."
                });

            if (!_otpManager.IsOtpValid(otp_request.CreatedAt))
                return Result.Fail<LoginWithQydhaRequest>(new()
                {
                    Code = ErrorType.OTPExceededTimeLimit,
                    Message = "OTP Exceed Time Limit"
                });
            return (await _loginWithQydhaRequestRepo.MarkRequestAsUsed(requestId)).MapTo(otp_request);
        })
        .OnSuccessAsync(async (request) => await _userRepo.GetByIdAsync(request.UserId))
        .OnSuccess((user) => Result.Ok(new Tuple<User, string>(user, _tokenManager.Generate(user.GetClaims()))));
    }

}