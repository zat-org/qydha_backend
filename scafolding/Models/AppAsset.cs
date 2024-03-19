using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class AppAsset
{
    public string AssetKey { get; set; } = null!;

    public string? AssetData { get; set; }
}
