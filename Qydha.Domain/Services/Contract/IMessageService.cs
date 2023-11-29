
namespace Qydha.Domain.Services.Contracts;

public interface IMessageService
{
    Task<Result> SendAsync(string phoneNum, string otp);
}
