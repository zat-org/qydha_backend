namespace Qydha.Domain.Services.Implementation;
public class NotificationService(INotificationRepo notificationRepo, ILogger<NotificationService> logger, IPushNotificationService pushNotificationService, IUserRepo userRepo, IFileService fileService, IOptions<NotificationImageSettings> imageSettings) : INotificationService
{
    #region injections
    private readonly INotificationRepo _notificationRepo = notificationRepo;
    private readonly IPushNotificationService _pushNotificationService = pushNotificationService;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly NotificationImageSettings _imageSettings = imageSettings.Value;
    private readonly IFileService _fileService = fileService;
    private readonly ILogger<NotificationService> _logger = logger;
    #endregion
    public async Task<Result<User>> SendToUser(Guid userId, NotificationData notification, Dictionary<string, string> templateValues)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes.OnSuccessAsync(async (user) => await SendToUser(user, notification, templateValues));
    }
    public async Task<Result<User>> SendToUser(User user, NotificationData notificationData, Dictionary<string, string> templateValues)
    {
        return (await _notificationRepo.CreateAndAssignToUser(user.Id, notificationData, templateValues))
        .OnSuccessAsync(async (notification) =>
        {
            if (!string.IsNullOrEmpty(user.FCMToken))
            {
                var notifyingRes = await _pushNotificationService.SendToToken(user.FCMToken, notification.Title, notification.Description);
                if (notifyingRes.IsFailed)
                    await _userRepo.UpdateUserFCMToken(user.Id, string.Empty);
            }
            return Result.Ok(user);
        });
    }
    public async Task<Result<User>> SendToUserPreDefinedNotification(Guid userId, int notificationId, Dictionary<string, string> templateValues)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes.OnSuccessAsync(async (user) => await SendToUserPreDefinedNotification(user, notificationId, templateValues));
    }
    public async Task<Result<User>> SendToUserPreDefinedNotification(User user, int notificationId, Dictionary<string, string> templateValues)
    {
        return (await _notificationRepo.AssignToUser(user.Id, notificationId, templateValues))
        .OnSuccessAsync(async (notification) =>
        {
            if (!string.IsNullOrEmpty(user.FCMToken))
            {
                var notifyingRes = await _pushNotificationService.SendToToken(user.FCMToken, notification.Title, notification.Description);
                if (notifyingRes.IsFailed)
                    await _userRepo.UpdateUserFCMToken(user.Id, string.Empty);
            }
            return Result.Ok(user);
        });
    }
    public async Task<Result<int>> SendToAllUsers(NotificationData notification, Dictionary<string, string> templateValues)
    {
        return (await _notificationRepo.CreateAndAssignToAllUsers(notification, templateValues))
        .OnSuccessAsync(async (tuple) =>
        {
            var effected = tuple.Item1;
            var notification = tuple.Item2;

            await _pushNotificationService.SendToAllUsers(notification);

            return Result.Ok(effected);
        });
    }
    public async Task<Result<Notification>> SendToAnonymousUsers(NotificationData notificationData)
    {
        return (await _notificationRepo.CreateAndAssignToAnonymousUsers(notificationData))
        .OnSuccessAsync(async (notification) =>
        {
            await _pushNotificationService.SendToAnonymousUsers(notification);
            return Result.Ok(notification);
        });
    }
    public async Task<Result> MarkAllNotificationsOfUserAsRead(Guid userId) =>
        (await _notificationRepo.MarkAllAsReadByUserIdAsync(userId)).ToResult();
    public async Task<Result> MarkNotificationAsRead(Guid userId, int notificationId) =>
        (await _notificationRepo.MarkAsReadByIdsAsync(userId, notificationId)).ToResult();
    public async Task<Result> DeleteNotification(Guid userId, int notificationId) =>
        (await _notificationRepo.DeleteByIdsAsync(userId, notificationId)).ToResult();
    public async Task<Result> DeleteAll(Guid userId) =>
        (await _notificationRepo.DeleteAllByUserIdAsync(userId)).ToResult();
    public async Task<Result<IEnumerable<Notification>>> GetByUserId(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null) =>
        await _notificationRepo.GetAllByUserId(userId, pageSize, pageNumber, isRead);
    public async Task<Result<IEnumerable<Notification>>> GetAllAnonymous(int pageSize = 10, int pageNumber = 1) =>
           await _notificationRepo.GetAllAnonymous(pageSize, pageNumber);
    public async Task<Result<FileData>> UploadNotificationImage(IFormFile file)
    {
        return await _fileService.UploadFile(_imageSettings.FolderPath, file);
    }
    public async Task<Result> ApplyAnonymousClick(int notificationId) =>
           (await _notificationRepo.ApplyAnonymousClickById(notificationId)).ToResult();
}