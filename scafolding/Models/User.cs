using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Name { get; set; }

    public string? PasswordHash { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public bool IsAnonymous { get; set; }

    public DateOnly? BirthDate { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool IsPhoneConfirmed { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string? AvatarUrl { get; set; }

    public string? AvatarPath { get; set; }

    public DateTime? ExpireDate { get; set; }

    public int FreeSubscriptionUsed { get; set; }

    public string? FcmToken { get; set; }

    public string? NormalizedUsername { get; set; }

    public string? NormalizedEmail { get; set; }

    public virtual ICollection<LoginWithQydhaRequest> LoginWithQydhaRequests { get; set; } = new List<LoginWithQydhaRequest>();

    public virtual ICollection<NotificationsUsersLink> NotificationsUsersLinks { get; set; } = new List<NotificationsUsersLink>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual UserBalootSetting? UserBalootSetting { get; set; }

    public virtual UserGeneralSetting? UserGeneralSetting { get; set; }

    public virtual UserHandSetting? UserHandSetting { get; set; }

    public virtual ICollection<UserPromoCode> UserPromoCodes { get; set; } = new List<UserPromoCode>();
}
