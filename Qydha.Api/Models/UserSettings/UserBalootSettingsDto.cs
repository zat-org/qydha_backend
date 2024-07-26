namespace Qydha.API.Models;

public class UserBalootSettingsDto
{
    public bool IsFlipped { get; set; } = false;
    public bool IsAdvancedRecording { get; set; } = false;
    public bool IsSakkahMashdodahMode { get; set; } = false;
    public bool ShowWhoWonDialogOnDraw { get; set; } = false;
    public bool IsNumbersSoundEnabled { get; set; } = false;
    public bool IsCommentsSoundEnabled { get; set; } = false;
    public bool IsEkakShown { get; set; } = false;
    public bool IsAklatShown { get; set; } = false;
    public int SakkasCount { get; set; } = 1;
}
