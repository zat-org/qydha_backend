using FirebaseAdmin.Messaging;
using Qydha.Domain.Settings;

namespace Qydha.Infrastructure.Services;

public class FCMService(IOptions<PushNotificationsSettings> pushNotificationsSettings) : IPushNotificationService
{
    private readonly PushNotificationsSettings _pushNotificationsSettings = pushNotificationsSettings.Value;

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
            return Result.Fail(new()
            {
                Code = ErrorType.InvalidFCMToken,
                Message = $" Invalid FCM Token Value : '{userToken}' "
            });
        var msg = CreateSingleTokenNotificationMessage(userToken, title, body);

        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
            return Result.Ok();
        }
        catch (FirebaseMessagingException exp)
        {
            // TODO :: Handle the MessagingErrorCode enum cases here 
            return Result.Fail(
                new()
                {
                    Code = ErrorType.FcmMessagingException,
                    Message = $"Error [FirebaseMessagingException] In Sending Push Notification to user , Message = {exp.Message} , Code = {exp.ErrorCode} , Messaging Error Code = {exp.MessagingErrorCode}"
                }
            );
        }
        catch (Exception exp)
        {
            return Result.Fail(
                new()
                {
                    Code = ErrorType.FcmRegularException,
                    Message = $"Error [Exception] In Sending Push Notification to user , Message = {exp.Message} , with FCM Token Value = {userToken}"
                }
            );
        }
    }
    private static async Task<Result> SendToTopic(string topicName, string title, string body)
    {
        if (string.IsNullOrEmpty(topicName))
            return Result.Fail(new()
            {
                Code = ErrorType.InvalidTopicName,
                Message = $" Invalid Topic Name Value : '{topicName}' "
            });
        var msg = CreateTopicNotificationMessage(topicName, title, body);
        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
            return Result.Ok();
        }
        catch (FirebaseMessagingException exp)
        {
            // TODO :: Handle the MessagingErrorCode enum cases here 

            return Result.Fail(
                new()
                {
                    Code = ErrorType.FcmMessagingException,
                    Message = $"Error [FirebaseMessagingException] In Sending Push Notification to topic , Message = {exp.Message} , Code = {exp.ErrorCode} , Messaging Error Code = {exp.MessagingErrorCode} , with Topic Name Value = '{topicName}'"
                }
            );
        }
        catch (Exception exp)
        {
            return Result.Fail(
                new()
                {
                    Code = ErrorType.FcmRegularException,
                    Message = $"Error [exception] In Sending Push Notification to user , Message = {exp.Message} , with Topic Name Value = '{topicName}'"
                }
            );
        }
    }

    public async Task<Result> SendToAnonymousUsers(Domain.Entities.Notification notification) =>
        await SendToTopic(_pushNotificationsSettings.ToAnonymousTopicName, notification.Title, notification.Description);
    public async Task<Result> SendToAllUsers(Domain.Entities.Notification notification) =>
        await SendToTopic(_pushNotificationsSettings.ToAllTopicName, notification.Title, notification.Description);
}

