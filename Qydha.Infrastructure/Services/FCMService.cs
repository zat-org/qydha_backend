using FirebaseAdmin.Messaging;
using Qydha.Domain.Settings;

namespace Qydha.Infrastructure.Services;

public class FCMService(IOptions<PushNotificationsSettings> pushNotificationsSettings, ILogger<FCMService> logger) : IPushNotificationService
{
    private readonly PushNotificationsSettings _pushNotificationsSettings = pushNotificationsSettings.Value;
    private readonly ILogger<FCMService> _logger = logger;
    private static Message CreateSingleTokenNotificationMessage(string userToken, string title, string body)
    {
        return new Message()
        {
            Token = userToken,
            Notification = new()
            {
                Title = title,
                Body = body,
            },
            Android = new AndroidConfig()
            {
                Notification = new AndroidNotification()
                {
                    Sound = "notification_alert",
                    ChannelId = "qydha",
                    DefaultSound = false,
                    Priority = NotificationPriority.MAX
                }
            },
            Apns = new ApnsConfig()
            {
                Aps = new Aps()
                {
                    CriticalSound = new CriticalSound()
                    {
                        Critical = true,
                        Name = "notification_alert.wav",
                        Volume = 1
                    }
                }
            }
        };
    }
    private static Message CreateTopicNotificationMessage(string topicName, string title, string body)
    {
        return new Message()
        {

            Topic = topicName,
            Notification = new()
            {
                Title = title,
                Body = body,
            },
            Android = new AndroidConfig()
            {
                Notification = new AndroidNotification()
                {
                    Sound = "notification_alert",
                    ChannelId = "qydha",
                    DefaultSound = false,
                    Priority = NotificationPriority.MAX
                }
            },
            Apns = new ApnsConfig()
            {
                Aps = new Aps()
                {
                    CriticalSound = new CriticalSound()
                    {
                        Critical = true,
                        Name = "notification_alert.wav",
                        Volume = 1
                    }
                }
            }
        };
    }

    public async Task<Result> SendToToken(string userToken, string title, string body)
    {
        if (string.IsNullOrEmpty(userToken))
            return Result.Fail(new NotifyingInvalidFCMTokenError(userToken));

        var msg = CreateSingleTokenNotificationMessage(userToken, title, body);
        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
            return Result.Ok();
        }
        catch (FirebaseMessagingException exp)
        {
            if (exp.MessagingErrorCode == MessagingErrorCode.Unregistered)
            {
                _logger.LogError("Trying to Notify Invalid FCM Token Value : {fcmToken}", userToken);
                return Result.Fail(new NotifyingInvalidFCMTokenError(userToken));
            }
            else
            {
                _logger.LogError("Error [FirebaseMessagingException] In Sending Push Notification to user with token : {fcmToken}, Message = {expMsg} , Code = {expCode} , Messaging Error Code = {expFcmMsgCode}", userToken, exp.Message, exp.ErrorCode, exp.MessagingErrorCode);
                return Result.Fail(new FCMError().CausedBy(exp));
            }
        }
        catch (Exception exp)
        {
            _logger.LogError("Error [Exception] In Sending Push Notification to user with token : {fcmToken}, Message = {expMsg} ", userToken, exp.Message);
            return Result.Fail(new FCMError().CausedBy(exp));
        }
    }
   
    private async Task<Result> SendToTopic(string topicName, string title, string body)
    {
        if (string.IsNullOrEmpty(topicName)) throw new ArgumentNullException(nameof(topicName));

        var msg = CreateTopicNotificationMessage(topicName, title, body);

        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
            _logger.LogInformation("this is the result from sending notification to topic using fcm : {fcmTopicResult} ", res);
            return Result.Ok();
        }
        catch (FirebaseMessagingException exp)
        {
            _logger.LogError("Error [FirebaseMessagingException] In Sending Push Notification to topic : {fcmTopic}, Message = {expMsg} , Code = {expCode} , Messaging Error Code = {expFcmMsgCode}", topicName, exp.Message, exp.ErrorCode, exp.MessagingErrorCode);
            return Result.Fail(new FCMError().CausedBy(exp));
        }
        catch (Exception exp)
        {
            _logger.LogError("Error [Exception] In Sending Push Notification to topic : {fcmTopic}, Message = {expMsg} ", topicName, exp.Message);
            return Result.Fail(new FCMError().CausedBy(exp));
        }
    }

    public async Task<Result> SendToAnonymousUsers(Domain.Entities.Notification notification) =>
        await SendToTopic(_pushNotificationsSettings.ToAnonymousTopicName, notification.Title, notification.Description);
    public async Task<Result> SendToAllUsers(Domain.Entities.Notification notification) =>
        await SendToTopic(_pushNotificationsSettings.ToAllTopicName, notification.Title, notification.Description);
}

