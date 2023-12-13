namespace Qydha.API.Mappers;
[Mapper]

public partial class AdminUserMapper
{
    [MapProperty(nameof(AdminUser.Created_At), nameof(AdminUserDto.CreatedAt))]
    [MapProperty(nameof(AdminUser.Role), nameof(AdminUserDto.Role))]
    public partial AdminUserDto AdminUserToAdminUserDto(AdminUser adminUser);
}
