namespace Qydha.Domain.Services.Contracts;

public interface IPushNotificationService
{
    Task<Result> SendToToken(string userToken, string title, string body);
    Task<Result> SendToTopic(string topicName, string title, string body);
    Task<Result> SendToTokens(IEnumerable<string> tokens, string title, string body);
}
