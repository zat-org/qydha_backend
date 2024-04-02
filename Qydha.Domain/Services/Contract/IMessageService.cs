
namespace Qydha.Domain.Services.Contracts;

public interface IMessageService
{
    Task<Result<string>> SendOtpAsync(string phoneNum, string username, string otp);
}
