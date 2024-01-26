namespace Qydha.API.Models;

public class UserLoginDto
{
    
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FCMToken { get; set; }

}
public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        string msg = "اسم المستخدم او كلمة السر غير صحيحة";
        RuleFor(r => r.Username)
        .Configure(config => config.CascadeMode = CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(msg);

        RuleFor(r => r.Password)
        .Configure(config => config.CascadeMode = CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(msg)
        .MinimumLength(6)
        .WithMessage(msg)
        .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).{6,}$")
        .WithMessage(msg);

        When(r => r.FCMToken is not null, () =>
        {
            RuleFor(r => r.FCMToken).FCM_Token("FCM_Token");
        });
    }
}