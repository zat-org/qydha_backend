using System.Net.Http.Json;
namespace Qydha.Infrastructure.Services;

public class UltraMsgService(IOptions<UltraMsgSettings> settings) : IMessageService
{
    private readonly UltraMsgSettings _ultraMsgSettings = settings.Value;

    public async Task<Result> SendOtpAsync(string phoneNum, string username, string otp)
    {
        using HttpClient httpClient = new();
        var data = new[]
        {
            new KeyValuePair<string, string>("token", _ultraMsgSettings.Token),
            new KeyValuePair<string, string>("to", phoneNum),
            new KeyValuePair<string, string>("body", @$"مرحبا بك *{username}*\nرمز التحقق *{otp}*"),
        };

        HttpResponseMessage response = await httpClient.PostAsync(
                new Uri($"{_ultraMsgSettings.ApiUrl}/{_ultraMsgSettings.Instance}/messages/chat"),
                new FormUrlEncodedContent(data));

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(
                new()
                {
                    Code = ErrorType.OTPPhoneSendingError,
                    Message = $"Failure From UltraMsg Service with status Code = {response.StatusCode} and content =  {response.Content.ToString() ?? "unknown Error from whatsapp."} "
                }
            );
        }
        return Result.Ok();
    }
}
