﻿namespace Qydha.Domain.Services.Implementation;

public class NotificationService : INotificationService
{
    private readonly INotificationRepo _notificationRepo;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IUserRepo _userRepo;
    private readonly NotificationsSettings _notificationsSettings;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepo notificationRepo, ILogger<NotificationService> logger, IPushNotificationService pushNotificationService, IUserRepo userRepo, IOptions<NotificationsSettings> notificationOptions)
    {
        _notificationRepo = notificationRepo;
        _pushNotificationService = pushNotificationService;
        _userRepo = userRepo;
        _notificationsSettings = notificationOptions.Value;
        _logger = logger;
    }


    public async Task<Result<User>> SendToUser(Notification notification)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(notification.UserId); 
        return getUserRes.OnSuccessAsync<User>(async (user) => await SendToUser(user, notification));
    }
    public async Task<Result<User>> SendToUser(User user, Notification notification)
    {
        return (await _notificationRepo.AddAsync<int>(notification))
        .OnSuccessAsync(async () =>
        {

            (await _pushNotificationService.SendToToken(user.FCMToken, notification.Title, notification.Description))
            .OnFailure((result) =>
            {
                //! Handle Error in send push notification to User with fcm;
                _logger.LogError(result.Error.ToString());

            });
            return Result.Ok(user);
        });
    }

    public async Task<Result<int>> SendToAllUsers(Notification notification)
    {
        return (await _notificationRepo.AddToUsersWithCriteria(notification))
        .OnSuccessAsync<int>(async (effected) =>
        {
            (await _pushNotificationService.SendToTopic(_notificationsSettings.ToAllTopicName, notification.Title, notification.Description))
            .OnFailure((result) =>
            {
                //! Handle Error in send push notification to User with fcm; 
                _logger.LogError(result.Error.ToString());
            });
            return Result.Ok(effected);
        });
    }

    public async Task<Result<int>> SendToGroupOfUsers(Notification notification, Func<User, bool> criteriaFunc)
    {
        return (await _userRepo.GetAllAsync())
        .OnSuccessAsync(async (users) =>
            (await _notificationRepo.AddToUsersWithByIds(notification, users.Where(criteriaFunc).Select(u => u.Id)))
                .MapTo<int, Tuple<IEnumerable<User>, int>>((effected) => new(users, effected)))
        .OnSuccessAsync(async (tuple) =>
        {
            (await _pushNotificationService.SendToTokens(tuple.Item1.Where(criteriaFunc).Select(u => u.FCMToken), notification.Title, notification.Description))
            .OnFailure((result) =>
            {
                //! TODO Handle Error In Send Tokens
                _logger.LogError(result.Error.ToString());
            });
            return Result.Ok(tuple.Item2);
        });
    }

    public async Task<Result> MarkNotificationAsRead(Guid userId, int notificationId) =>
        await _notificationRepo.MarkNotificationAsRead(userId, notificationId);

    public async Task<Result> DeleteNotification(Guid userId, int notificationId) =>
        await _notificationRepo.DeleteByIdAndUserIdAsync(userId, notificationId);
    public async Task<Result> DeleteAll(Guid userId) =>
        await _notificationRepo.DeleteAllByUserIdAsync(userId);
    public async Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null) =>
        await _notificationRepo.GetAllNotificationsOfUserById(userId, pageSize, pageNumber, isRead);

}