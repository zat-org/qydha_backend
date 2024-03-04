namespace Qydha.Domain.Services.Contracts;

public interface IAuthService
{
    Task<Result<Tuple<User, string>>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId);
    Task<Result<Tuple<User, string>>> Login(string username, string password, string? fcm_token);

    Task<Result<PhoneAuthenticationRequest>> RequestPhoneAuthentication(string phone);
    Task<Result<Tuple<User, string>>> ConfirmPhoneAuthentication(Guid requestId, string otp, string? fcmToken);

    Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken, Guid? userId);
    Task<Result> Logout(Guid userId);

    Task<Result<LoginWithQydhaRequest>> SendOtpToLoginWithQydha(string username, string serviceConsumerName);
    Task<Result<Tuple<User, string>>> ConfirmLoginWithQydha(Guid requestId, string otpCode);
}
