namespace Qydha.Domain.Services.Contracts;

public interface IAuthService
{
    Task<Result<Tuple<User, UserGeneralSettings, string>>> LoginAsAnonymousAsync();
    Task<Result<Tuple<User, string>>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId);
    Task<Result<Tuple<User, UserGeneralSettings, string>>> Login(string username, string password, string? fcm_token);

    Task<Result<PhoneAuthenticationRequest>> RequestPhoneAuthentication(string phone);
    Task<Result<Tuple<User, string>>> ConfirmPhoneAuthentication(Guid requestId, string otp, string? fcmToken);

    Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken, Guid? userId);
    Task<Result> Logout(Guid userId);
}
