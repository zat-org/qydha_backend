﻿namespace Qydha.Domain.Entities;

public class UserBalootSettings
{
    public Guid UserId { get; set; }

    public bool IsFlipped { get; set; } = false;

    public bool IsAdvancedRecording { get; set; } = false;

    public bool IsSakkahMashdodahMode { get; set; } = false;

    public bool ShowWhoWonDialogOnDraw { get; set; } = false;

    public bool IsNumbersSoundEnabled { get; set; } = true;

    public bool IsCommentsSoundEnabled { get; set; } = true;
    public virtual User User { get; set; } = null!;

}
