using Newtonsoft.Json;

namespace Qydha.Domain.Entities;
public class NotificationData
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string ActionPath { get; set; } = string.Empty;

    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;

    public NotificationDataPayload Payload = new();

    public NotificationVisibility Visibility { get; set; } = NotificationVisibility.Private;
    public NotificationSendingMechanism SendingMechanism { get; set; } = NotificationSendingMechanism.Manual;
    public int AnonymousClicks { get; set; }
    public Dictionary<string, string> TemplateValues = [];
    public virtual ICollection<NotificationUserLink> NotificationUserLinks { get; set; } = [];
    public static string ReplacePlaceholders(string template, Dictionary<string, string> templateValues)
    {
        foreach (var kvp in templateValues)
        {
            template = template.Replace("{" + kvp.Key + "}", kvp.Value);
        }
        return template;
    }
}

public class NotificationDataPayload
{
    public FileData? Image { get; set; } = null;
}
