namespace Qydha.Domain.MediatorNotifications;

public class AddTransactionNotification(Guid userId, TransactionType type) : INotification
{
    public Guid UserId { get; } = userId;
    public TransactionType Type { get; } = type;
}
