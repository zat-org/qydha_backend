namespace Qydha.API.Models;

public class TokenWithUserDataDto
{
    public string? Token { get; set; } 
    public GetUserDto? User { get; set; } 
}
