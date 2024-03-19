using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class UpdatePhoneRequest
{
    public Guid Id { get; set; }

    public string Phone { get; set; } = null!;

    public string Otp { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid UserId { get; set; }
}
