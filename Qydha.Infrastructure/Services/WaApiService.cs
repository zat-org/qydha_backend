using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace Qydha.Infrastructure.Services;

public class WaApiService(IOptions<WaApiSettings> settings, IHttpClientFactory clientFactory, ILogger<WaApiService> logger)
{
    private readonly WaApiSettings _waApiSettings = settings.Value;
    private readonly IHttpClientFactory _clientFactory = clientFactory;
    private readonly ILogger<WaApiService> _logger = logger;
    public async Task<Result<string>> SendOtpAsync(string phoneNum, string username, string otp, int instanceId)
    {
        if (phoneNum.StartsWith("+")) //"966533331913@c.us"
            phoneNum = phoneNum[1..];
        if (!phoneNum.EndsWith("@c.us"))
            phoneNum += "@c.us";

        var httpClient = _clientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _waApiSettings.Token);
        try
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                    new Uri($"{_waApiSettings.ApiUrl}/{instanceId}/client/action/send-message"),
                    new
                    {
                        chatId = phoneNum,
                        message = @$"مرحبا بك *{username}*\nرمز التحقق *{otp}*"
                    });

            string jsonResponse = await response.Content.ReadAsStringAsync();
            RootObject responseObject = JsonConvert.DeserializeObject<RootObject>(jsonResponse) ?? throw new Exception($"can't serialize the response from WaApi Service with body : {jsonResponse} ");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogCritical("WaApi has Failure Status Code {statusCode} and response body : {response}", response.StatusCode, jsonResponse);
                return Result.Fail<string>(new()
                {
                    Code = ErrorType.WaApiUnknownError,
                    Message = $"UnExpected error occurred "
                });
            }
            if (responseObject.Data.Status == DataStatus.error)
            {
                _logger.LogCritical("WaApi has Success Status Code But with Error Status In Body :=> response body : {response}", responseObject);
                if (responseObject.Data.Message == "instance not ready")
                {
                    return Result.Fail<string>(new()
                    {
                        Code = ErrorType.WaApiInstanceNotReady,
                        Message = $"WaApi Instance NotReady"
                    });
                }
                else
                {
                    return Result.Fail<string>(new()
                    {
                        Code = ErrorType.WaApiUnknownError,
                        Message = $"UnExpected error occurred "
                    });
                }
            }
            return Result.Ok($"WhatsApp:WaApi:{instanceId}");
        }

        catch (Exception ex)
        {
            _logger.LogCritical("WaApi has Exception {exp}", ex);
            return Result.Fail<string>(new()
            {
                Code = ErrorType.WaApiUnknownError,
                Message = $"UnExpected ERROR occurred : {ex.Message}"
            });
        }

    }

    internal class WaApiData
    {
        public DataStatus Status { get; set; }
        public string InstanceId { get; set; } = null!;
        public string? Message { get; set; }
        public string? Explanation { get; set; }
        public string? InstanceStatus { get; set; }
        public object? Data { get; set; }
    }
    internal enum DataStatus
    {
        success,
        error
    }
    internal class Links
    {
        public string Self { get; set; } = null!;
    }
    internal class RootObject
    {
        public WaApiData Data { get; set; } = null!;
        public Links Links { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
