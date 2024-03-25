
namespace Qydha.Domain.Services.Implementation;

public class LoginWithQydhaOtpSenderAsNotification(INotificationService notificationService) : ILoginWithQydhaOtpSenderService
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<User>> SendOtpAsync(User user, string otp, string serviceConsumerName)
    {
        return await _notificationService.SendToUserPreDefinedNotification(user, SystemDefaultNotifications.LoginWithQydha, new Dictionary<string, string>(){
            {"ServiceName" , serviceConsumerName} ,
            {"Otp" , otp}
        });
    }
}
