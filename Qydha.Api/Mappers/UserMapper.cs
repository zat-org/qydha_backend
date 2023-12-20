
namespace Qydha.API.Mappers;
[Mapper]
public partial class UserMapper
{
    [MapProperty(nameof(User.CreatedAt), nameof(GetUserDto.CreatedOn))]
    public partial GetUserDto UserToUserDto(User user);

    public partial UserGeneralSettingsDto UserGeneralSettingsToDto(UserGeneralSettings userSettings);
    public partial UserGeneralSettings DtoToUserGeneralSettings(UserGeneralSettingsDto dto);

    private IEnumerable<string> JsonToStringArray(Json<IEnumerable<string>> jsonStringArr)
    {
        return jsonStringArr.Value ?? new List<string>();
    }

}