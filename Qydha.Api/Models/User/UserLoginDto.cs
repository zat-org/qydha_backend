namespace Qydha.API.Models;

public class UserLoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(r => r.Username).Username("اسم المستخدم").WithMessage("'{PropertyName}' غير صحيح");
        RuleFor(r => r.Password).Password("كلمة المرور").WithMessage("'{PropertyName}' غير صحيح");
    }
}