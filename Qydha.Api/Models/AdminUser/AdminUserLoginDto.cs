namespace Qydha.API.Models;

public class AdminUserLoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

}
public class AdminUserLoginDtoValidator : AbstractValidator<AdminUserLoginDto>
{
    public AdminUserLoginDtoValidator()
    {
        string msg = "اسم المستخدم او كلمة السر غير صحيحة";
        RuleFor(r => r.Username)
        .Configure(config => config.CascadeMode = CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(msg);
        // .Length(2, 25)
        // .WithMessage(msg)
        // .Matches("^[a-zA-Z][a-zA-Z0-9_.-]*$")
        // .WithMessage(msg);

        RuleFor(r => r.Password)
        .Configure(config => config.CascadeMode = CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(msg);
        // .MinimumLength(6)
        // .WithMessage(msg)
        // .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).{6,}$")
        // .WithMessage(msg);

    }
}