namespace Qydha.API.Models;

public class UserRegisterDTO
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? FCM_Token { get; set; }

}
public class UserRegisterDTOValidator : AbstractValidator<UserRegisterDTO>
{
    public UserRegisterDTOValidator()
    {
        RuleFor(r => r.Username).Username("اسم المتسخدم");
        RuleFor(r => r.Password).Password("كلمة المرور");
        RuleFor(r => r.Phone).Phone("رقم الجوال");
        When(r => r.FCM_Token is not null, () =>
        {
            RuleFor(r => r.FCM_Token).FCM_Token("FCM_Token");
        });
    }
}
