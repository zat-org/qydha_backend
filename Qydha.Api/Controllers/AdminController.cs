namespace Qydha.API.Controllers;

[ApiController]
[Route("/admin")]
public class AdminController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _authService.Login(dto.Username, dto.Password, true, null))
        .Resolve(
            (authUserModel) =>
            {
                Response.Cookies.AddRefreshToken(authUserModel.RefreshToken, authUserModel.RefreshTokenExpirationDate);
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { adminUser = mapper.UserToUserDto(authUserModel.User), token = authUserModel.JwtToken, authUserModel.RefreshTokenExpirationDate },
                    message = "Logged in successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }
}
