namespace Qydha.Domain.Services.Contracts;

public interface IPushNotificationService
{
    Task<Result> SendToToken(string userToken, string title, string body);
    Task<Result> SendToAnonymousUsers(Notification notification);
    Task<Result> SendToAllUsers(Notification notification);
}
