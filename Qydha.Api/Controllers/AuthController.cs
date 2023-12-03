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
        return (await _authService.RegisterAsync(dto.Username, dto.Password, dto.Phone, dto.FCMToken, null))
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


    [Authorize]
    [TypeFilter(typeof(AuthFilter))]
    [HttpPost("register-anonymous/")]
    public async Task<IActionResult> RegisterAnonymous([FromBody] UserRegisterDTO dto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _authService.RegisterAsync(dto.Username, dto.Password, dto.Phone, dto.FCMToken, userId))
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
        return (await _authService.Login(dto.Username, dto.Password, dto.FCMToken))
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
            (result) => BadRequest(new
            {
                Code = ErrorCodes.InvalidCredentials,
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

    [Authorize]
    [TypeFilter(typeof(AuthFilter))]
    [HttpPost("logout/")]
    public async Task<IActionResult> Logout()
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        return (await _authService.Logout(userId))
        .Handle<IActionResult>(
            () => Ok(new { data = new { }, message = "User logged out successfully." }),
            BadRequest
        );
    }

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

