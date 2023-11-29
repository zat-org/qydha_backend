using FirebaseAdmin.Messaging;

namespace Qydha.Infrastructure.Services;

public class FCMService : IPushNotificationService
{
    public async Task<Result> SendToToken(string userToken, string title, string body)
    {
        var msg = new Message()
        {
            Token = userToken,
            Notification = new()
            {
                Title = title,
                Body = body
            }
        };
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
                    Code = exp.ErrorCode.ToString(),
                    Message = $"Error In Sending Push Notification to user , Message = {exp.Message} , Code = {exp.ErrorCode} , Messaging Error Code = {exp.MessagingErrorCode}"
                }
            );
        }
    }

    public async Task<Result> SendToTopic(string topicName, string title, string body)
    {
        var msg = new Message()
        {
            Topic = topicName,
            Notification = new()
            {
                Title = title,
                Body = body
            }
        };
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
                    Code = exp.ErrorCode.ToString(),
                    Message = $"Error In Sending Push Notification to topic , Message = {exp.Message} , Code = {exp.ErrorCode} , Messaging Error Code = {exp.MessagingErrorCode}"
                }
            );
        }
    }

    public async Task<Result> SendToTokens(IEnumerable<string> tokens, string title, string body)
    {
        var msg = new MulticastMessage()
        {
            Tokens = tokens.ToList(),
            Notification = new()
            {
                Title = title,
                Body = body
            }
        };
        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(msg);
            return Result.Ok();
        }
        catch (FirebaseMessagingException exp)
        {
            // TODO :: Handle the MessagingErrorCode enum cases here 
            return Result.Fail(
                new()
                {
                    Code = exp.ErrorCode.ToString(),
                    Message = $"Error In Sending Push Notification to Tokens , Message = {exp.Message} , Code = {exp.ErrorCode} , Messaging Error Code = {exp.MessagingErrorCode}"
                });
        }
    }
}

