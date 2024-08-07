namespace Qydha.API.Models;
public class UsersPage(List<GetUserDto> items, int count, int pageNumber, int pageSize)
    : Page<GetUserDto>(items, count, pageNumber, pageSize)
{ }