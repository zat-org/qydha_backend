
namespace Qydha.API.Models;

public class ChangeEmailDto
{
    public string Password { get; set; } = null!;
    public string NewEmail { get; set; } = null!;

}
public class ChangeEmailDtoValidator : AbstractValidator<ChangeEmailDto>
{
    public ChangeEmailDtoValidator()
    {
        RuleFor(r => r.Password).Password("كلمة المرور");
        RuleFor(r => r.NewEmail).NotEmpty().WithName("البريد الالكترونى").EmailAddress();
    }
}