namespace Qydha.Domain.Services.Contracts;

public interface ILoginWithQydhaOtpSenderService
{
    Task<Result<User>> SendOtpAsync(User user, string otp, string serviceConsumerName);
}
