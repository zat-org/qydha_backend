namespace Qydha.Domain.Mappers;
[Mapper]
public partial class UserStreamMapper
{
    [MapProperty(nameof(User.CreatedAt), nameof(GetUserDto.CreatedOn))]
    public partial GetUserDto UserToUserDto(User user);
}
