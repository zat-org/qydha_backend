namespace Qydha.Domain.MediatorNotifications;

public class UserDataChangedNotification(User user) : INotification
{
    public User User { get; } = user;
}
