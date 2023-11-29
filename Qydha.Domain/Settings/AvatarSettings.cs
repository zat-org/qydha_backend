
namespace Qydha.Domain.Settings;

public class AvatarSettings
{

    public int MaxBytes { get; set; }

    public ICollection<string> AcceptedFileTypes { get; set; } = new List<string>();

    public string FolderPath { get; set; } = "avatars/";


}
