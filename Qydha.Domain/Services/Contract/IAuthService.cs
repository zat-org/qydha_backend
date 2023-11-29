namespace Qydha.Domain.Services.Contracts;

public interface IAuthService
{
    Task<Result<string>> LoginAsAnonymousAsync();

    Task<Result<Tuple<User, string>>> ConfirmRegistrationWithPhone(string otpCode, Guid requestId);

    Task<Result<Tuple<User, string>>> Login(string username, string password);

    Task<Result<RegistrationOTPRequest>> RegisterAsync(string username, string password, string phone, string? fcmToken, Guid? userId);

    Task<Result> Logout(Guid userId);
}
