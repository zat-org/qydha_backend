
namespace Qydha.Domain.Services.Contracts;

public interface IMessageService
{
    Task<Result> SendOtpAsync(string phoneNum, string username, string otp);
}
