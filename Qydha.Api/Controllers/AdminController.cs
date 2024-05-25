namespace Qydha.API.Controllers;

[ApiController]
[Route("/admin")]
public class AdminController(IAdminUserService adminUserService) : ControllerBase
{
    private readonly IAdminUserService _adminUserService = adminUserService;

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] AdminUserLoginDto dto)
    {
        return (await _adminUserService.Login(dto.Username, dto.Password))
        .Resolve(
            (tuple) =>
            {
                var mapper = new AdminUserMapper();
                return Ok(new
                {
                    data = new { adminUser = mapper.AdminUserToAdminUserDto(tuple.adminUser), token = tuple.jwtToken },
                    message = "Logged in successfully."
                });
            }
        , HttpContext.TraceIdentifier);
    }


    [Auth(SystemUserRoles.Admin)]
    [HttpPatch("change-password/")]
    public async Task<IActionResult> ChangePassword([FromBody] AdminUserChangePasswordDto dto)
    {
        AdminUser adminUser = (AdminUser)HttpContext.Items["User"]!;

        return (await _adminUserService.ChangePassword(adminUser.Id, dto.OldPassword, dto.NewPassword))
        .Resolve((adminUser) =>
            {
                var mapper = new AdminUserMapper();
                return Ok(new
                {
                    data = new { adminUser = mapper.AdminUserToAdminUserDto(adminUser) },
                    message = "Password updated successfully."
                });
            }, HttpContext.TraceIdentifier);
    }
}
