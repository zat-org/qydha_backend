namespace Qydha.Domain.Settings;

public class SubscriptionSetting
{
    public string FreeSubscriptionName { get; set; } = "free_30";

    public int FreeSubscriptionsAllowed { get; set; } = 1;
    public int NumberOfDaysInOneSubscription { get; set; } = 30;

}
