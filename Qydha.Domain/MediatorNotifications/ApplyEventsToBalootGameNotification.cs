namespace Qydha.Domain.MediatorNotifications;

public class ApplyEventsToBalootGameNotification(Guid userId, BalootGame game) : INotification
{
    public Guid UserId { get; } = userId;
    public BalootGame Game { get; } = game;
}
