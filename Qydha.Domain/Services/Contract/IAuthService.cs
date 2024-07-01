namespace Qydha.Domain.Services.Contracts;

public interface IAuthService
{
    Task<Result<(User user, string jwtToken)>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId);
    Task<Result<(User user, string jwtToken)>> Login(string username, string password, bool asAdmin = false, string? fcm_token = null);
    Task<Result<PhoneAuthenticationRequest>> RequestPhoneAuthentication(string phone);
    Task<Result<(User user, string jwtToken)>> ConfirmPhoneAuthentication(Guid requestId, string otp, string? fcmToken);
    Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken);
    Task<Result> Logout(Guid userId);
    Task<Result<LoginWithQydhaRequest>> SendOtpToLoginWithQydha(string username, string serviceConsumerName);
    Task<Result<(User user, string jwtToken)>> ConfirmLoginWithQydha(Guid requestId, string otpCode);
}
