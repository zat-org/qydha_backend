namespace Qydha.Domain.Enums;

[Flags]
public enum SystemUserRoles
{
    RegularUser = 1,
    AnonymousUser = 2,
    User = 3,
    SuperAdmin = 4,
    StaffAdmin = 8,
    Admin = 12,
    All = 15
}

public enum AdminType
{
    SuperAdmin,
    StaffAdmin
}