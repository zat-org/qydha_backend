namespace Qydha.API.Controllers;

[ApiController]
[Route("/admin")]
public class AdminController(IAdminUserService adminUserService) : ControllerBase
{
    private readonly IAdminUserService _adminUserService = adminUserService;

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _adminUserService.Login(dto.Username, dto.Password))
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
