using Newtonsoft.Json;

namespace Qydha.Domain.Entities;

public class Notification
{
    public Notification() { }
    public long Id { get; set; }
    private string _title = string.Empty;
    public string Title
    {
        get
        {
            return ReplacePlaceholders(_title, _templateValues);
        }
        set
        {
            _title = value;
        }
    }
    private string _description = string.Empty;
    public string Description
    {
        get
        {
            return ReplacePlaceholders(_description, _templateValues);
        }
        set
        {
            _description = value;
        }
    }

    private string _actionPath = string.Empty;
    public string ActionPath
    {
        get
        {
            return ReplacePlaceholders(_actionPath, _templateValues);
        }
        set
        {
            _actionPath = value;
        }
    }
    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;
    public NotificationDataPayload Payload = new();
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
    private Dictionary<string, string> _templateValues = [];
    public List<Dictionary<string, string>> TemplateValues
    {
        set
        {
            _templateValues = [];
            foreach (var dictionary in value)
            {
                foreach (var kvp in dictionary)
                {
                    _templateValues[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    public static string ReplacePlaceholders(string template, Dictionary<string, string> templateValues)
    {
        foreach (var kvp in templateValues)
        {
            template = template.Replace("{" + kvp.Key + "}", kvp.Value);
        }
        return template;
    }
}

public static class SystemDefaultNotifications
{
    public const int Register = 1;
    public const int MakePurchase = 2;
    public const int GetTicket = 3;
    public const int UseTicket = 4;
    public const int UseInfluencerCode = 5;
    public const int LoginWithQydha = 6;
    public const int RefundPurchase = 7;
}