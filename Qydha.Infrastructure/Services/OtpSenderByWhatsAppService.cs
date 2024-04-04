namespace Qydha.Infrastructure.Services;

public class OtpSenderByWhatsAppService(WaApiService waApiService, WaApiInstancesTracker instancesTracker, ILogger<OtpSenderByWhatsAppService> logger) : IMessageService
{
    //  WhatsAppService whatsAppService,
    private readonly WaApiService _waApiService = waApiService;
    // private readonly WhatsAppService _whatsAppService = whatsAppService;
    private readonly WaApiInstancesTracker _instancesTracker = instancesTracker;
    private readonly ILogger<OtpSenderByWhatsAppService> _logger = logger;

    public async Task<Result<string>> SendOtpAsync(string phoneNum, string username, string otp)
    {
        var instance = _instancesTracker.DequeueInstance();
        if (instance is null)
        {
            return Result.Fail<string>(new() { Code = ErrorType.OTPPhoneSendingError, Message = "can't send the otp now , please try again later" });
            // return await _whatsAppService.SendOtpAsync(phoneNum, username, otp);
        }
        else
        {
            return (await _waApiService.SendOtpAsync(phoneNum, username, otp, instance.InstanceId))
            .Handle(
            (sender) =>
            {
                _instancesTracker.EnqueueInstance(instance);
                return Result.Ok(sender);
            },
            (error) =>
            {
                _logger.LogCritical("WaApi Instance is out of Service with Instance id :=> {id}", instance.InstanceId);
                // return await _whatsAppService.SendOtpAsync(phoneNum, username, otp);
                return Result.Fail<string>(new() { Code = ErrorType.OTPPhoneSendingError, Message = "can't send the otp now , please try again later" });
            });
        }
    }
}

