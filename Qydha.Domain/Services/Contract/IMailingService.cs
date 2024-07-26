namespace Qydha.Domain.Services.Contracts;

public interface IMailingService
{
    Task<Result<string>> SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null);
    Task<string> GenerateConfirmEmailBody(string otp, string requestId, User user);
    Task<Result<(string otp, Guid requestId, string sender)>> SendOtpToEmailAsync(string mailTo, string otp, User user);

}
