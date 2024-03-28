using System.Net.Http.Json;
namespace Qydha.Infrastructure.Services;

public class WhatsAppService(IOptions<WhatsAppSettings> whatsSettings, IHttpClientFactory clientFactory, ILogger<WaApiService> logger) : IMessageService
{
    private readonly WhatsAppSettings _whatsSettings = whatsSettings.Value;
    private readonly IHttpClientFactory _clientFactory = clientFactory;
    private readonly ILogger<WaApiService> _logger = logger;

    public async Task<Result> SendOtpAsync(string phoneNum, string username, string otp)
    {
        var httpClient = _clientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _whatsSettings.Token);

        var body = new
        {
            messaging_product = "whatsapp",
            to = phoneNum,
            type = "template",
            template = new
            {
                name = "qydha_otp",
                language = new
                {
                    code = "ar"
                },
                components = new object[]
                {
                    new
                    {
                        type = "body",
                        parameters = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = otp
                            }
                        }
                    },
                    new
                    {
                        type = "button",
                        sub_type = "url",
                        index = "0",
                        parameters = new object[]
                        {
                            new
                            {
                                type = "payload",
                                payload = otp
                            }
                        }
                    }
                }
            }
        };
        try
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(new Uri(_whatsSettings.ApiUrl), body);
            if (!response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                object responseObject = JsonConvert.DeserializeObject<object>(jsonResponse) ?? throw new Exception($"can't serialize the response from WhatsApp Service with body : {jsonResponse} ");
                _logger.LogCritical("WhatsApp has Failure Status Code {statusCode} and response body : {response}", response.StatusCode, jsonResponse);
                return Result.Fail(
                    new()
                    {
                        Code = ErrorType.OTPPhoneSendingError,
                        Message = $"Failure From WhatsApp Service"
                    }
                );
            }
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("WhatsApp Service has Exception {exp}", ex);
            return Result.Fail(new()
            {
                Code = ErrorType.OTPPhoneSendingError,
                Message = $"UnExpected ERROR occurred on WhatsApp Service"
            });
        }
    }
}
