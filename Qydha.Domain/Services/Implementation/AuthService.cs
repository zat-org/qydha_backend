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


    public async Task<Result<AuthenticatedUserModel>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId)
    {

        return (await _registrationOTPRequestRepo.GetByIdAsync(requestId))
        .OnSuccess(otpReq => otpReq.IsRequestValidToUse(_otpManager, otpCode).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _userRepo.IsUsernameAndPhoneAvailable(otpReq.Username, otpReq.Phone)).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) =>
        {
            var user = User.CreateUserFromRegisterRequest(otpReq);
            var refreshToken = _tokenManager.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            return (await _userRepo.AddAsync(user))
                    .ToResult(user =>
                        new AuthenticatedUserModel(user, _tokenManager.GenerateJwtToken(user), refreshToken.Token, refreshToken.ExpireAt));
        })
        .OnSuccessAsync(async (authUserModel) =>
        {
            await _mediator.Publish(new UserRegistrationNotification(authUserModel.User));
            return Result.Ok(authUserModel);
        })
        .OnSuccessAsync(async (authUserModel) => (await _registrationOTPRequestRepo.MarkRequestAsUsed(requestId, authUserModel.User.Id)).ToResult(authUserModel));
    }

    public async Task<Result<AuthenticatedUserModel>> Login(string username, string password, bool asAdmin = false, string? fcmToken = null)
    {
        return (await _userRepo.CheckUserCredentials(username, password))
        .OnSuccess((user) =>
        {
            if (asAdmin && !user.Roles.Any(r => r == UserRoles.SuperAdmin || r == UserRoles.StaffAdmin))
                return Result.Fail(new ForbiddenError());
            return Result.Ok(user);
        })
        .OnSuccessAsync(async (user) =>
        {
            if (!string.IsNullOrEmpty(fcmToken))
                user.FCMToken = fcmToken;

            user.LastLogin = DateTimeOffset.UtcNow;

            RefreshToken refreshToken;
            if (user.RefreshTokens.Any(r => r.IsActive))
                refreshToken = user.RefreshTokens.First(r => r.IsActive);
            else
            {
                refreshToken = _tokenManager.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
            }
            return (await _userRepo.UpdateAsync(user)).ToResult((user) => (user, refreshToken));
        })
        .OnSuccess((tuple) =>
        {
            var authUserModel = new AuthenticatedUserModel(tuple.user, _tokenManager.GenerateJwtToken(tuple.user), tuple.refreshToken.Token, tuple.refreshToken.ExpireAt);
            return Result.Ok(authUserModel);
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
            return (await _messageService.SendOtpAsync(phone, username, otp)).ToResult((sender) => (sender, otp));
        })
        .OnSuccessAsync(async (tuple) =>
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            RegistrationOTPRequest registrationOTP = new(username, phone, passwordHash, tuple.otp, fcmToken, tuple.sender);
            return await _registrationOTPRequestRepo.AddAsync(registrationOTP);
        });
    }

    public async Task<Result> Logout(Guid userId) =>
        await _userRepo.UpdateUserFCMToken(userId, string.Empty);

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

    public async Task<Result<AuthenticatedUserModel>> ConfirmPhoneAuthentication(Guid requestId, string otpCode, string? fcmToken)
    {
        return (await _phoneAuthenticationRequestRepo.GetByIdAsync(requestId))
            .OnSuccess((otpReq) => otpReq.IsValidToConfirmPhoneAuthUsingIt(_otpManager, otpCode).ToResult(otpReq))
            .OnSuccessAsync(async (otpReq) => (await _phoneAuthenticationRequestRepo.MarkRequestAsUsed(requestId)).ToResult(otpReq))
            .OnSuccessAsync(async (otpReq) => await _userRepo.GetByIdAsync(otpReq.UserId, withTracking: true))
            .OnSuccessAsync(async (user) =>
            {
                if (!string.IsNullOrEmpty(fcmToken))
                    user.FCMToken = fcmToken;

                user.LastLogin = DateTimeOffset.UtcNow;

                RefreshToken refreshToken;
                if (user.RefreshTokens.Any(r => r.IsActive))
                    refreshToken = user.RefreshTokens.First(r => r.IsActive);
                else
                    refreshToken = _tokenManager.GenerateRefreshToken();
                return (await _userRepo.UpdateAsync(user)).ToResult((user) => (user, refreshToken));
            })
            .OnSuccess((tuple) => Result.Ok(new
                AuthenticatedUserModel(tuple.user, _tokenManager.GenerateJwtToken(tuple.user), tuple.refreshToken.Token, tuple.refreshToken.ExpireAt))
            );
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

    public async Task<Result<User>> ConfirmLoginWithQydha(Guid requestId, string otpCode)
    {
        return (await _loginWithQydhaRequestRepo.GetByIdAsync(requestId))
        .OnSuccess((otpReq) => otpReq.IsRequestValidToUse(_otpManager, otpCode).ToResult(otpReq))
        .OnSuccessAsync(async (otpReq) => (await _loginWithQydhaRequestRepo.MarkRequestAsUsed(requestId)).ToResult(otpReq))
        .OnSuccessAsync(async (request) => await _userRepo.GetByIdAsync(request.UserId));
    }
    public Result<AuthenticatedUserModel> RefreshToken(string jwtToken, string refreshToken)
    {
        return _tokenManager.GetPrincipalFromExpiredToken(jwtToken)
            .OnSuccess(principal => principal.GetUserIdentifier())
            .OnSuccessAsync((userId) => _userRepo.GetByIdAsync(userId, withTracking: true))
            .OnSuccessAsync(async user =>
            {
                var refreshTokenEntity = user.RefreshTokens.FirstOrDefault(r => r.Token == refreshToken && r.IsActive);
                if (refreshTokenEntity == null) return Result.Fail(new InvalidRefreshTokenError());
                refreshTokenEntity.RevokedAt = DateTimeOffset.UtcNow;
                RefreshToken newRefreshToken = _tokenManager.GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                return (await _userRepo.UpdateAsync(user))
                    .ToResult((user) => (user, newRefreshToken));
            })
            .OnSuccess((tuple) =>
            {
                return Result.Ok(new AuthenticatedUserModel(tuple.user, _tokenManager.GenerateJwtToken(tuple.user), tuple.newRefreshToken.Token, tuple.newRefreshToken.ExpireAt));
            });
    }
}