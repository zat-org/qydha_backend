namespace Qydha.Domain.Settings;

public class BookSettings
{
    public int MaxBytes { get; set; }

    public ICollection<string> AcceptedFileTypes { get; set; } = new List<string>();

    public string FolderPath { get; set; } = "qydha_assets/books/";
}
