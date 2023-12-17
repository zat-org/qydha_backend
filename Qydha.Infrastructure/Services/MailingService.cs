using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Qydha.Infrastructure.Services;

public class MailingService : IMailingService
{
    private readonly EmailSettings _mailSettings;
    public MailingService(IOptions<EmailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }
    public async Task<Result> SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null)
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
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new()
            {
                Code = ErrorCodes.EmailSendingError,
                Message = $"can't send the email using mailKit with message : {e.Message}"
            });
        }
    }

    public async Task<string> GenerateConfirmEmailBody(string otp, string requestId, User user)
    {
        string fileName = "confirmEmailCode.html";
        string path = Path.Combine(Environment.CurrentDirectory, @"Templates", fileName);
        StreamReader streamReader = new(path);
        string mailText = await streamReader.ReadToEndAsync();
        streamReader.Close();
        string styledOtp = string.Join("", otp.Split("").Select((d) =>
        {
            return $"<span class='character'>{d}</span>";
        }));
        return mailText.Replace("[code]", otp);
    }

}
