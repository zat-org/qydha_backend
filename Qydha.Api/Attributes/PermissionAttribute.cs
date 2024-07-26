namespace Qydha.API.Attributes;
public class PermissionAttribute(ServiceAccountPermission permission) : AuthorizeAttribute
{
    public ServiceAccountPermission Permission { get; } = permission;
}
