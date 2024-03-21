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
        .Handle<RegistrationOTPRequest, IActionResult>(
            (req) => Ok(
                new
                {
                    data = new
                    {
                        RequestId = req.Id,
                    },
                    Message = "otp sent successfully."
                }),
            BadRequest
        );
    }

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _authService.Login(dto.Username, dto.Password, dto.FCMToken))
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                User user = tuple.Item1;
                string token = tuple.Item2;
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(user),
                        token
                    },
                    message = "Logged In successfully."
                });
            },
            (result) => BadRequest(new Error()
            {
                Code = ErrorType.InvalidCredentials,
                Message = "اسم المستخدم او كلمة السر غير صحيحة"
            })
        );
    }

    [HttpPost("confirm-registration-with-phone/")]
    public async Task<IActionResult> ConfirmRegistrationWithPhone([FromBody] ConfirmPhoneDto dto)
    {
        return (await _authService.ConfirmRegistrationWithPhone(dto.Code, dto.RequestId))
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(tuple.Item1), token = tuple.Item2 },
                    message = "Register in successfully."
                });
            },
            BadRequest
        );
    }


    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
    {
        return (await _authService.RequestPhoneAuthentication(dto.Phone!))
        .Handle<PhoneAuthenticationRequest, IActionResult>(
            (request) => Ok(new { data = new { RequestId = request.Id }, message = "Otp sent successfully." })
            , BadRequest);
    }

    [HttpPost("confirm-forget-password")]
    public async Task<IActionResult> ConfirmForgetPassword([FromBody] ConfirmForgetPasswordDto dto)
    {
        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCMToken))
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.Item1),
                        token = tuple.Item2
                    },
                    message = "user logged in successfully."
                });
            }
            , BadRequest);
    }


    [HttpPost("login-with-phone")]
    public async Task<IActionResult> LoginWithPhone([FromBody] LoginWithPhoneDto dto)
    {
        return (await _authService.RequestPhoneAuthentication(dto.Phone!))
        .Handle<PhoneAuthenticationRequest, IActionResult>(
            (request) => Ok(new { data = new { RequestId = request.Id }, message = "Otp sent successfully." })
            , BadRequest);
    }

    [HttpPost("confirm-login-with-phone")]
    public async Task<IActionResult> ConfirmLoginWithPhone([FromBody] ConfirmLoginWithPhoneDto dto)
    {

        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCMToken))
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.Item1),
                        token = tuple.Item2
                    },
                    message = "user logged in successfully."
                });
            }
            , BadRequest);
    }


    [Auth(SystemUserRoles.RegularUser)]
    [HttpPost("logout/")]
    public async Task<IActionResult> Logout()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _authService.Logout(user.Id))
        .Handle<IActionResult>(
            () => Ok(new { data = new { }, message = "User logged out successfully." }),
            BadRequest
        );
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPost("login-with-qydha")]
    public async Task<IActionResult> LoginWithQydha(LoginWithQydhaDto dto)
    {
        AdminUser serviceConsumer = (AdminUser)HttpContext.Items["User"]!;

        return (await _authService.SendOtpToLoginWithQydha(dto.Username, "زات"))
        .Handle<LoginWithQydhaRequest, IActionResult>(
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
            },
            BadRequest
        );
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpPost("confirm-login-with-qydha")]
    public async Task<IActionResult> ConfirmLoginWithQydha(ConfirmLoginWithQydhaDto dto)
    {
        AdminUser serviceConsumer = (AdminUser)HttpContext.Items["User"]!;

        return (await _authService.ConfirmLoginWithQydha(dto.RequestId, dto.Otp))
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(tuple.Item1),
                        token = tuple.Item2
                    },
                    message = "user logged in successfully."
                });
            }
            , BadRequest);
    }
}

