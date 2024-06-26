﻿namespace Qydha.Domain.Services.Implementation;
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
        return getUserRes.OnSuccessAsync<User>(async (user) => await SendToUser(user, notification, templateValues));
    }
    public async Task<Result<User>> SendToUser(User user, NotificationData notificationData, Dictionary<string, string> templateValues)
    {
        return (await _notificationRepo.CreateAndAssignToUser(user.Id, notificationData, templateValues))
        .OnSuccessAsync(async (notification) =>
        {
            if (!string.IsNullOrEmpty(user.FCMToken))
                (await _pushNotificationService.SendToToken(user.FCMToken, notification.Title, notification.Description))
                .OnFailure((err) =>
                {
                    //! Handle Error in send push notification to User with fcm;
                    _logger.LogError("Error In Send push Notification To user with ID :{userId} with code : {errorCode} and Message : {errorMsg}", user.Id, err.Code, err.Message);
                    return err;
                });
            return Result.Ok(user);
        });
    }
    public async Task<Result<User>> SendToUserPreDefinedNotification(Guid userId, int notificationId, Dictionary<string, string> templateValues)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes.OnSuccessAsync<User>(async (user) => await SendToUserPreDefinedNotification(user, notificationId, templateValues));
    }
    public async Task<Result<User>> SendToUserPreDefinedNotification(User user, int notificationId, Dictionary<string, string> templateValues)
    {
        return (await _notificationRepo.AssignToUser(user.Id, notificationId, templateValues))
        .OnSuccessAsync(async (notification) =>
        {
            if (!string.IsNullOrEmpty(user.FCMToken))
            {
                (await _pushNotificationService.SendToToken(user.FCMToken, notification.Title, notification.Description))
                .OnFailure((err) =>
                {
                    //! Handle Error in send push notification to User with fcm;
                    _logger.LogError("Error In Send push Notification To user with ID :{userId} with code : {errorCode} and Message : {errorMsg}", user.Id, err.Code, err.Message);
                    return err;
                });
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
            (await _pushNotificationService.SendToAllUsers(notification))
            .OnFailure((err) =>
            {
                //! Handle Error in send push notification to User with fcm; 
                _logger.LogError("Error In Send push Notification To All users with code : {errorCode} and Message : {errorMsg}", err.Code, err.Message);
                return err;
            });
            return Result.Ok(effected);
        });
    }
    public async Task<Result<Notification>> SendToAnonymousUsers(NotificationData notificationData)
    {
        return (await _notificationRepo.CreateAndAssignToAnonymousUsers(notificationData))
        .OnSuccessAsync<Notification>(async (notification) =>
        {
            (await _pushNotificationService.SendToAnonymousUsers(notification))
            .OnFailure((err) =>
            {
                //! Handle Error in send push notification to User with fcm;
                _logger.LogError("Error In Send push Notification To Anonymous users with code : {errorCode} and Message : {errorMsg}", err.Code, err.Message);
                return err;
            });
            return Result.Ok(notification);
        });
    }
    public async Task<Result> MarkAllNotificationsOfUserAsRead(Guid userId) =>
        await _notificationRepo.MarkAllAsReadByUserIdAsync(userId);
    public async Task<Result> MarkNotificationAsRead(Guid userId, int notificationId) =>
        await _notificationRepo.MarkAsReadByIdsAsync(userId, notificationId);
    public async Task<Result> DeleteNotification(Guid userId, int notificationId) =>
        await _notificationRepo.DeleteByIdsAsync(userId, notificationId);
    public async Task<Result> DeleteAll(Guid userId) =>
        await _notificationRepo.DeleteAllByUserIdAsync(userId);
    public async Task<Result<IEnumerable<Notification>>> GetByUserId(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null) =>
        await _notificationRepo.GetAllByUserId(userId, pageSize, pageNumber, isRead);
    public async Task<Result<IEnumerable<Notification>>> GetAllAnonymous(int pageSize = 10, int pageNumber = 1) =>
           await _notificationRepo.GetAllAnonymous(pageSize, pageNumber);
    public async Task<Result<FileData>> UploadNotificationImage(IFormFile file)
    {
        return (await _fileService.UploadFile(_imageSettings.FolderPath, file))
                    .OnFailure((err) =>
                    {
                        _logger.LogError("Can't Upload Notification Image with file Data = {fileData} with Error code = {errorCode},  Message = {errorMsg} ", new { file.Length, name = file.FileName, file.ContentType }, err.Code, err.Message);
                        return new()
                        {
                            Code = ErrorType.FileUploadError,
                            Message = "حدث عطل اثناء حفظ الصورة برجاء المحاولة مرة اخري"
                        };
                    });
    }
    public async Task<Result> ApplyAnonymousClick(int notificationId) =>
            await _notificationRepo.ApplyAnonymousClickById(notificationId);
}