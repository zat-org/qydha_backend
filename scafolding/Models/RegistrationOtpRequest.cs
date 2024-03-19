using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class RegistrationOtpRequest
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Otp { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid? UserId { get; set; }

    public string? FcmToken { get; set; }
}
