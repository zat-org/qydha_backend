namespace Qydha.Domain.Entities;

public class Notification
{
    public int Notification_Id { get; set; }
    public Guid User_Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Read_At { get; set; }
    public DateTime Created_At { get; set; } = DateTime.UtcNow;
    public string Action_Path { get; set; } = string.Empty;
    public NotificationActionType Action_Type { get; set; } = NotificationActionType.NoAction;

    public static Notification CreateRegisterNotification(User user)
    {
        return new Notification()
        {
            Title = "مرحبا بك في تطبيق قيدها",
            Description = "يمكنك الان الاستمتاع بجميع مميزات الاشتراك الذهبى مجانا ولمدة شهر كامل. سارع بالانضمام الان.",
            Action_Path = "",
            Action_Type = NotificationActionType.NoAction,
            User_Id = user.Id
        };
    }
}

