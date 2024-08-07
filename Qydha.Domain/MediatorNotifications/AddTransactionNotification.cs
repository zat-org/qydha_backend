namespace Qydha.Domain.MediatorNotifications;

public class AddTransactionNotification(User user, TransactionType type) : INotification
{
    public User User { get; } = user;
    public TransactionType Type { get; } = type;
}
