namespace Qydha.Domain.Services.Contracts;

public interface IAuthService
{
    Task<Result<AuthenticatedUserModel>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId);
    Task<Result<AuthenticatedUserModel>> Login(string username, string password, List<UserRoles> mustBeInRole, string? fcm_token = null);
    Task<Result<PhoneAuthenticationRequest>> RequestPhoneAuthentication(string phone);
    Task<Result<AuthenticatedUserModel>> ConfirmPhoneAuthentication(Guid requestId, string otp, string? fcmToken);
    Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken);
    Task<Result> Logout(Guid userId);
    Task<Result<LoginWithQydhaRequest>> SendOtpToLoginWithQydha(string username, string serviceConsumerName);
    Task<Result<User>> ConfirmLoginWithQydha(Guid requestId, string otpCode);
    Result<AuthenticatedUserModel> RefreshToken(string jwtToken, string refreshToken);
}
