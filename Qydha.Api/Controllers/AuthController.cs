namespace Qydha.API.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register/")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
    {
        return (await _authService.RegisterAsync(dto.Username, dto.Password, dto.Phone, dto.FCMToken))
        .Resolve(
            (req) => Ok(
                new
                {
                    data = new
                    {
                        RequestId = req.Id,
                    },
                    Message = "otp sent successfully."
                }
            ));
    }

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _authService.Login(dto.Username, dto.Password, dto.FCMToken))
        .Resolve(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.user),
                        token = tuple.jwtToken
                    },
                    message = "Logged In successfully."
                });
            });
    }

    [HttpPost("confirm-registration-with-phone/")]
    public async Task<IActionResult> ConfirmRegistrationWithPhone([FromBody] ConfirmPhoneDto dto)
    {
        return (await _authService.ConfirmRegistrationWithPhone(dto.Code, dto.RequestId))
        .Resolve(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(tuple.user), token = tuple.jwtToken },
                    message = "Register in successfully."
                });
            });
    }


    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
    {
        return (await _authService.RequestPhoneAuthentication(dto.Phone!))
        .Resolve((request) => Ok(new { data = new { RequestId = request.Id }, message = "Otp sent successfully." }));
    }

    [HttpPost("confirm-forget-password")]
    public async Task<IActionResult> ConfirmForgetPassword([FromBody] ConfirmForgetPasswordDto dto)
    {
        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCMToken))
        .Resolve(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.user),
                        token = tuple.jwtToken
                    },
                    message = "user logged in successfully."
                });
            });
    }


    [HttpPost("login-with-phone")]
    public async Task<IActionResult> LoginWithPhone([FromBody] LoginWithPhoneDto dto)
    {
        return (await _authService.RequestPhoneAuthentication(dto.Phone!))
        .Resolve((request) => Ok(new { data = new { RequestId = request.Id }, message = "Otp sent successfully." }));
    }

    [HttpPost("confirm-login-with-phone")]
    public async Task<IActionResult> ConfirmLoginWithPhone([FromBody] ConfirmLoginWithPhoneDto dto)
    {

        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCMToken))
        .Resolve(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.user),
                        token = tuple.jwtToken
                    },
                    message = "user logged in successfully."
                });
            });
    }


    [Auth(SystemUserRoles.RegularUser)]
    [HttpPost("logout/")]
    public async Task<IActionResult> Logout()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _authService.Logout(user.Id))
        .Resolve(
            () => Ok(new { data = new { }, message = "User logged out successfully." })
        );
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPost("login-with-qydha")]
    public async Task<IActionResult> LoginWithQydha(LoginWithQydhaDto dto)
    {
        AdminUser serviceConsumer = (AdminUser)HttpContext.Items["User"]!;

        return (await _authService.SendOtpToLoginWithQydha(dto.Username, "زات"))
        .Resolve(
            (loginReq) =>
            {
                return Ok(new
                {
                    data = new
                    {
                        id = loginReq.Id,
                    },
                    message = "otp sent successfully to the user."
                });
            });
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPost("confirm-login-with-qydha")]
    public async Task<IActionResult> ConfirmLoginWithQydha(ConfirmLoginWithQydhaDto dto)
    {
        AdminUser serviceConsumer = (AdminUser)HttpContext.Items["User"]!;

        return (await _authService.ConfirmLoginWithQydha(dto.RequestId, dto.Otp))
        .Resolve(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.user),
                        token = tuple.jwtToken
                    },
                    message = "user logged in successfully."
                });
            });
    }
}

