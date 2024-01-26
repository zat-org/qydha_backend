﻿using System.Net.Http.Json;
namespace Qydha.Infrastructure.Services;

public class WhatsAppService(IOptions<WhatsAppSettings> whatsSettings) : IMessageService
{
    private readonly WhatsAppSettings _whatsSettings = whatsSettings.Value;

    public async Task<Result> SendOtpAsync(string phoneNum, string username, string otp)
    {
        using HttpClient httpClient = new();
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

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(new Uri(_whatsSettings.ApiUrl), body);
        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(
                new()
                {
                    Code = ErrorType.OTPPhoneSendingError,
                    Message = $"Failure From WhatsApp Service with status Code = {response.StatusCode} and content =  {response.Content.ToString() ?? "unknown Error from whatsapp."} "
                }
            );
        }

        return Result.Ok();
    }
}
