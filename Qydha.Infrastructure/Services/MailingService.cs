using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Qydha.Infrastructure.Services;

public class MailingService(IOptions<EmailSettings> mailSettings, ILogger<MailingService> logger) : IMailingService
{
    private readonly EmailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<MailingService> _logger = logger;

    public async Task<Result<string>> SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null)
    {
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Email),
            Subject = subject
        };

        email.To.Add(MailboxAddress.Parse(mailTo));
        var builder = new BodyBuilder();

        if (attachments is not null)
        {
            byte[] fileBytes;
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }

        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
            var resStr = await smtp.SendAsync(email);
            return Result.Ok($"Email:MailKit:{_mailSettings.Email}");
        }
        catch (Exception exp)
        {
            _logger.LogCritical("Error in Sending Email using mailKit to {email} with exception message : {expMsg}", mailTo, exp.Message);
            return Result.Fail(new OtpEmailSendingError("mailKit").CausedBy(exp));
        }
    }

    public async Task<string> GenerateConfirmEmailBody(string otp, string requestId, User user)
    {
        string fileName = "confirmEmailCode.html";
        string path = Path.Combine(Environment.CurrentDirectory, @"Templates", fileName);
        StreamReader streamReader = new(path);
        string mailText = await streamReader.ReadToEndAsync();
        streamReader.Close();
        string styledOtp = string.Join("", otp.ToCharArray().Select((d) =>
        {
            return $"<span class='character'>{d}</span>";
        }));
        Console.WriteLine(styledOtp);
        return mailText.Replace("[code]", styledOtp);
    }

    public async Task<Result<(string otp, Guid requestId, string sender)>> SendOtpToEmailAsync(string newEmail, string otp, User user)
    {
        Guid requestId = Guid.NewGuid();
        var emailSubject = "تأكيد البريد الالكتروني لحساب تطبيق قيدها";
        var emailBody = await GenerateConfirmEmailBody(otp, requestId.ToString(), user);
        return (await SendEmailAsync(newEmail, emailSubject, emailBody)).ToResult((sender) => (otp, requestId, sender));
    }
}
