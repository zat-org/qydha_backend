
namespace Qydha.Domain.Services.Implementation;

public class LoginWithQydhaOtpSenderAsNotification(INotificationService notificationService) : ILoginWithQydhaOtpSenderService
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<User>> SendOtpAsync(User user, string otp, string serviceConsumerName)
    {
        Dictionary<string, object> payload = [];
        return await _notificationService.SendToUser(user, new()
        {
            Title = $"تسجيل دخول الى {serviceConsumerName} ",
            Description = $"رمز الدخول هو {otp} تستطيع استخدامه لتسجيل الدخول علي {serviceConsumerName} باستخدام حسابك بتطبيق قيدها",
            ActionPath = "_",
            ActionType = NotificationActionType.NoAction,
            CreatedAt = DateTime.UtcNow,
            Visibility = NotificationVisibility.Private,
            Payload = payload,
        });
    }
}
 