namespace Qydha.API.Mappers;
[Mapper]
public partial class UserMapper
{
    [MapProperty(nameof(User.CreatedAt), nameof(GetUserDto.CreatedOn))]
    public partial GetUserDto UserToUserDto(User user);
}