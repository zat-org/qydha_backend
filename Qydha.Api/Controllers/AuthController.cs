namespace Qydha.API.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login-anonymous/")]
    public async Task<IActionResult> LoginAsAnonymous()
    {
        return (await _authService.LoginAsAnonymousAsync())
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(tuple.Item1), token = tuple.Item2 },
                    message = "Anonymous account created successfully."
                });
            }
            , BadRequest);
    }

    [HttpPost("register/")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
    {
        return (await _authService.RegisterAsync(dto.Username, dto.Password, dto.Phone, dto.FCM_Token, null))
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


    [Authorization(AuthZUserType.User)]
    [HttpPost("register-anonymous/")]
    public async Task<IActionResult> RegisterAnonymous([FromBody] UserRegisterDTO dto)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _authService.RegisterAsync(dto.Username, dto.Password, dto.Phone, dto.FCM_Token, user.Id))
        .Handle<RegistrationOTPRequest, IActionResult>((req) => Ok(
            new
            {
                data = new
                {
                    RequestId = req.Id,
                },
                Message = "otp sent successfully."
            }),
        BadRequest);
    }

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _authService.Login(dto.Username, dto.Password, dto.FCM_Token))
        .Handle<Tuple<User, string>, IActionResult>(
            (tuple) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new { user = mapper.UserToUserDto(tuple.Item1), token = tuple.Item2 },
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
        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCM_Token))
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

        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCM_Token))
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


    [Authorization(AuthZUserType.User)]
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


    [Authorization(AuthZUserType.User)]
    [HttpGet("test")]
    public IActionResult TestDeploy()
    {
        return Ok(new { message = "Deployed. ✔️✔️" });
    }



    [HttpGet("throwError")]
    public IActionResult ThrowError()
    {
        throw new InvalidOperationException();
    }

}

