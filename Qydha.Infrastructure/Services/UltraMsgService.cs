using System.Net.Http.Json;
namespace Qydha.Infrastructure.Services;

public class UltraMsgService(IOptions<UltraMsgSettings> settings) : IMessageService
{
    private readonly UltraMsgSettings _ultraMsgSettings = settings.Value;

    public async Task<Result> SendAsync(string phoneNum, string otp)
    {
        using HttpClient httpClient = new();

        var data = new[]
        {
            new KeyValuePair<string, string>("token", _ultraMsgSettings.Token),
            new KeyValuePair<string, string>("to", phoneNum),
            new KeyValuePair<string, string>("body", @$"
             *{otp}* هو كود التحقق الخاص بك. للحفاظ على امانك، لا تشارك هذا الكود مع أي شخص.

                _تنتهى صلاحية هذا الكود خلال 6 دقائق_
            "),
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
