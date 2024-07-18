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
            ), HttpContext.TraceIdentifier);
    }

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        return (await _authService.Login(dto.Username, dto.Password, asAdmin: false, dto.FCMToken))
        .Resolve(
            (authUserModel) =>
            {
                Response.Cookies.AddRefreshToken(authUserModel.RefreshToken, authUserModel.RefreshTokenExpirationDate);
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(authUserModel.User),
                        token = authUserModel.JwtToken,
                        authUserModel.RefreshTokenExpirationDate
                    },
                    message = "Logged In successfully."
                });
            }, HttpContext.TraceIdentifier);
    }

    [HttpPost("confirm-registration-with-phone/")]
    public async Task<IActionResult> ConfirmRegistrationWithPhone([FromBody] ConfirmPhoneDto dto)
    {
        return (await _authService.ConfirmRegistrationWithPhone(dto.Code, dto.RequestId))
        .Resolve(
           (authUserModel) =>
            {
                Response.Cookies.AddRefreshToken(authUserModel.RefreshToken, authUserModel.RefreshTokenExpirationDate);
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(authUserModel.User),
                        token = authUserModel.JwtToken,
                        authUserModel.RefreshTokenExpirationDate
                    },
                    message = "Register in successfully."
                });
            }, HttpContext.TraceIdentifier);
    }


    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
    {
        return (await _authService.RequestPhoneAuthentication(dto.Phone!))
        .Resolve((request) => Ok(new { data = new { RequestId = request.Id }, message = "Otp sent successfully." }), HttpContext.TraceIdentifier);
    }

    [HttpPost("confirm-forget-password")]
    public async Task<IActionResult> ConfirmForgetPassword([FromBody] ConfirmForgetPasswordDto dto)
    {
        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCMToken))
        .Resolve(
            (authUserModel) =>
            {
                Response.Cookies.AddRefreshToken(authUserModel.RefreshToken, authUserModel.RefreshTokenExpirationDate);
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(authUserModel.User),
                        token = authUserModel.JwtToken,
                        authUserModel.RefreshTokenExpirationDate
                    },
                    message = "user logged in successfully."
                });
            }, HttpContext.TraceIdentifier);
    }


    [HttpPost("login-with-phone")]
    public async Task<IActionResult> LoginWithPhone([FromBody] LoginWithPhoneDto dto)
    {
        return (await _authService.RequestPhoneAuthentication(dto.Phone!))
        .Resolve((request) => Ok(new { data = new { RequestId = request.Id }, message = "Otp sent successfully." }), HttpContext.TraceIdentifier);
    }

    [HttpPost("confirm-login-with-phone")]
    public async Task<IActionResult> ConfirmLoginWithPhone([FromBody] ConfirmLoginWithPhoneDto dto)
    {

        return (await _authService.ConfirmPhoneAuthentication(dto.RequestId, dto.Code, dto.FCMToken))
        .Resolve(
            (authUserModel) =>
            {
                Response.Cookies.AddRefreshToken(authUserModel.RefreshToken, authUserModel.RefreshTokenExpirationDate);
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(authUserModel.User),
                        token = authUserModel.JwtToken,
                        authUserModel.RefreshTokenExpirationDate
                    },
                    message = "user logged in successfully."
                });
            }, HttpContext.TraceIdentifier);
    }


    [Authorize(Roles = RoleConstants.UserWithAnyRole)]
    [HttpPost("logout/")]
    public IActionResult Logout()
    {
        return HttpContext.User.GetUserIdentifier()
        .OnSuccessAsync(_authService.Logout)
        .Resolve(
            () => Ok(new { data = new { }, message = "User logged out successfully." })
        , HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.ServiceAccountPermission)]
    [Permission(ServiceAccountPermission.LoginWithQydha)]
    [HttpPost("login-with-qydha")]
    public async Task<IActionResult> LoginWithQydha(LoginWithQydhaDto dto)
    {
        // AdminUser serviceConsumer = (AdminUser)HttpContext.Items["User"]!;

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
            }, HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.ServiceAccountPermission)]
    [Permission(ServiceAccountPermission.LoginWithQydha)]
    [HttpPost("confirm-login-with-qydha")]
    public async Task<IActionResult> ConfirmLoginWithQydha(ConfirmLoginWithQydhaDto dto)
    {
        return (await _authService.ConfirmLoginWithQydha(dto.RequestId, dto.Otp))
        .Resolve(
            (user) =>
            {
                var mapper = new UserMapper();
                return Ok(new
                {
                    data = new
                    {
                        user = mapper.UserToUserDto(user),
                    },
                    message = "user logged in successfully."
                });
            }, HttpContext.TraceIdentifier);
    }


    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var refreshToken = string.IsNullOrWhiteSpace(dto.RefreshToken) ?
            Request.Cookies[CookiesExtensions.RefreshTokenCookieName] : dto.RefreshToken;
        if (string.IsNullOrEmpty(refreshToken))
            return new InvalidRefreshTokenError("Refresh token not provided").Handle(HttpContext.TraceIdentifier);
        return _authService.RefreshToken(dto.JwtToken, refreshToken)
        .Resolve(res =>
        {
            Response.Cookies.AddRefreshToken(res.RefreshToken, res.RefreshTokenExpirationDate);
            return Ok(new
            {
                data = new
                {
                    token = res.JwtToken,
                    res.RefreshTokenExpirationDate
                },
                message = "new access token created."
            });
        }
         , HttpContext.TraceIdentifier);

    }
}

