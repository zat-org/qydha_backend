namespace Qydha.Domain.MediatorNotifications;

public class ApplyEventsToBalootGameNotification(Guid userId, BalootGame game, BalootGameEventEffect effect) : INotification
{
    public Guid UserId { get; } = userId;
    public BalootGame Game { get; } = game;
    public BalootGameEventEffect Effect { get; } = effect;
}
