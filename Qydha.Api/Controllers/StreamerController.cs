using Google.Api;

namespace Qydha.API.Controllers;

[ApiController]
[Route("/streamer")]
public class StreamerController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _authService.Login(dto.Username, dto.Password, [UserRoles.Streamer], null))
        .Resolve(
            (authUserModel) =>
            {
                // Response.Cookies.AddRefreshToken(authUserModel.RefreshToken, authUserModel.RefreshTokenExpirationDate);
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        adminUser = mapper.UserToUserDto(authUserModel.User),
                        authUserModel.JwtToken,
                        authUserModel.RefreshTokenExpirationDate,
                        authUserModel.RefreshToken,
                    },
                    message = "Logged in successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }

}
