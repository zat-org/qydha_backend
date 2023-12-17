namespace Qydha.Domain.Services.Contracts;

public interface IMailingService
{
    Task<Result> SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null);
    Task<string> GenerateConfirmEmailBody(string otp, string requestId, User user);
}
