namespace Qydha.Domain.Settings;

public class RegisterGiftSetting
{
    public bool IsWorking { get; set; }
    public string CodeName { get; set; } = null!;
    public int NumberOfGiftedDays { get; set; }
    public int ExpireAfterInDays { get; set; }
}
