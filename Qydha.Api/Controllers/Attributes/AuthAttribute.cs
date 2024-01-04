namespace Qydha.API.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class AuthAttribute(SystemUserRoles sysRole = SystemUserRoles.All) : Attribute
{
    public SystemUserRoles Role { get; } = sysRole;

}
