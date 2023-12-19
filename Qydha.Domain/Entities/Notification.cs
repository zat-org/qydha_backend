namespace Qydha.Domain.Entities;
[Table("notification")]
[NotFoundError(ErrorType.NotificationNotFound)]

public class Notification
{
    [Key]
    [Column("notification_id")]
    public int Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("title")]
    public string Title { get; set; } = string.Empty;
    [Column("description")]
    public string Description { get; set; } = string.Empty;
    [Column("read_at")]
    public DateTime? ReadAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("action_path")]
    public string ActionPath { get; set; } = string.Empty;
    [Column("action_type")]
    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;

    public static Notification CreateRegisterNotification(User user)
    {
        return new Notification()
        {
            Title = "مرحبا بك في تطبيق قيدها",
            Description = "يمكنك الان الاستمتاع بجميع مميزات الاشتراك الذهبى مجانا ولمدة شهر كامل. سارع بالانضمام الان.",
            ActionPath = "",
            ActionType = NotificationActionType.NoAction,
            UserId = user.Id
        };
    }
    public static Notification CreatePurchaseNotification(Purchase p)
    {
        return new Notification()
        {
            Title = "شكراً لاشتراكك في قيدها",
            Description = "نتمنى لك تجربة رائعة",
            ActionPath = "",
            ActionType = NotificationActionType.NoAction,
            UserId = p.UserId
        };
    }
    public static Notification CreatePromoCodeNotification(UserPromoCode promo)
    {
        return new Notification()
        {
            Title = "وصلتك هدية !!",
            Description = "شيك على التذاكر في قسم المتجر 🎉",
            ActionPath = "",
            ActionType = NotificationActionType.NoAction,
            UserId = promo.UserId
        };
    }

}

