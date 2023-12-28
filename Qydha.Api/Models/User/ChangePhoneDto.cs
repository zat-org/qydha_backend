namespace Qydha.API.Models;


public class ChangePhoneDto
{
    public string Password { get; set; } = null!;
    public string NewPhone { get; set; } = null!;
}
public class ChangePhoneDtoValidator : AbstractValidator<ChangePhoneDto>
{
    public ChangePhoneDtoValidator()
    {
        RuleFor(r => r.Password).NotEmpty().WithName("كلمة المرور");
        RuleFor(r => r.NewPhone).Phone("رقم الجوال");
    }
}