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
        .Handle<Tuple<AdminUser, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new AdminUserMapper();
                return Ok(new
                {
                    data = new { adminUser = mapper.AdminUserToAdminUserDto(tuple.Item1), token = tuple.Item2 },
                    message = "Logged in successfully."
                });
            },
            (result) => BadRequest(new Error()
            {
                Code = ErrorType.InvalidCredentials,
                Message = "اسم المستخدم او كلمة السر غير صحيحة"
            })
        );
    }


    [Authorization(AuthZUserType.Admin)]
    [HttpPatch("change-password/")]
    public async Task<IActionResult> ChangePassword([FromBody] AdminUserChangePasswordDto dto)
    {
        AdminUser adminUser = (AdminUser)HttpContext.Items["User"]!;

        return (await _adminUserService.ChangePassword(adminUser.Id, dto.OldPassword, dto.NewPassword))
        .Handle<AdminUser, IActionResult>((adminUser) =>
            {
                var mapper = new AdminUserMapper();
                return Ok(new
                {
                    data = new { adminUser = mapper.AdminUserToAdminUserDto(adminUser) },
                    message = "Password updated successfully."
                });
            },
            BadRequest);
    }
}
