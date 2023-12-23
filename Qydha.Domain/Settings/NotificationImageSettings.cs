
namespace Qydha.Domain.Settings;

public class NotificationImageSettings
{
    public int MaxBytes { get; set; }

    public ICollection<string> AcceptedFileTypes { get; set; } = new List<string>();

    public string FolderPath { get; set; } = "dev-notification-images/";
}
