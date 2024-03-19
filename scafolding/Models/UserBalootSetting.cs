using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class UserBalootSetting
{
    public Guid UserId { get; set; }

    public bool? IsFlipped { get; set; }

    public bool? IsAdvancedRecording { get; set; }

    public bool? IsSakkahMashdodahMode { get; set; }

    public bool? ShowWhoWonDialogOnDraw { get; set; }

    public bool? IsNumbersSoundEnabled { get; set; }

    public bool? IsCommentsSoundEnabled { get; set; }

    public virtual User User { get; set; } = null!;
}
