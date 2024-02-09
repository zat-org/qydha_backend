namespace Qydha.Domain.MediatorNotifications;

public class AddPromoCodeNotification(Guid userId) : INotification
{
    public Guid UserId { get; } = userId;
}
