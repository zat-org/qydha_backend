namespace Qydha.API.Models;


public class ChangeUsernameDto
{
    public string Password { get; set; } = null!;
    public string NewUsername { get; set; } = null!;
}

public class ChangeUsernameDtoValidator : AbstractValidator<ChangeUsernameDto>
{
    public ChangeUsernameDtoValidator()
    {
        RuleFor(r => r.Password).NotEmpty().WithName("كلمة المرور");
        RuleFor(r => r.NewUsername).Username("اسم المستخدم");
    }
}