
namespace Qydha.API.Mappers;
[Mapper]
public partial class UserMapper
{
    [MapProperty(nameof(User.CreatedAt), nameof(GetUserDto.CreatedOn))]
    public partial GetUserDto UserToUserDto(User user);

    public UsersPage PageListToUserPageDto(PagedList<User> users)
    {
        return new UsersPage(users.Select(UserToUserDto).ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize);
    }
    public partial UpdateUserDto UserToUpdateUserDto(User user);
    public partial void ApplyUpdateUserDtoToUser(UpdateUserDto dto, User user);


    public partial UserGeneralSettingsDto UserGeneralSettingsToDto(UserGeneralSettings userSettings);
    public partial UserHandSettingsDto UserHandSettingsToDto(UserHandSettings userHandSettings);
    public partial UserBalootSettingsDto UserBalootSettingsToDto(UserBalootSettings userBalootSettings);

    public partial void DtoToUserGeneralSettings(UserGeneralSettingsDto dto, UserGeneralSettings userSettings);
    public partial void DtoToUserHandSettings(UserHandSettingsDto dto, UserHandSettings userHandSettings);
    public partial void DtoToUserBalootSettings(UserBalootSettingsDto dto, UserBalootSettings userBalootSettings);

    public partial UserGeneralSettings DtoToUserGeneralSettings(UserGeneralSettingsDto dto);

}