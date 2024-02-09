namespace Qydha.Domain.MediatorNotifications;

public class UserRegistrationNotification(User user) : INotification
{
    public User User { get; } = user;
}
