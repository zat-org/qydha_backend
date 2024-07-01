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
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { adminUser = mapper.UserToUserDto(tuple.user), token = tuple.jwtToken },
                    message = "Logged in successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }
}
