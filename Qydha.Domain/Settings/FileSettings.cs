namespace Qydha.Domain.Settings;

public class FileSettings
{
    public int MaxBytes { get; set; }

    public ICollection<string> AcceptedFileTypes { get; set; } = null!;

    public string FolderPath { get; set; } = null!;

}
