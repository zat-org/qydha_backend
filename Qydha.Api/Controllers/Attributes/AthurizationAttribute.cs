namespace Qydha.API.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class AuthorizationAttribute(AuthZUserType role) : Attribute
{
    public AuthZUserType Role { get; } = role;
}
