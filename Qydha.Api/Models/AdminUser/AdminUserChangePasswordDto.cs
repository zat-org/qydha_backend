namespace Qydha.API.Models;

public class AdminUserChangePasswordDto
{

    public string OldPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!;

}

public class AdminUserChangePasswordDtoValidator : AbstractValidator<AdminUserChangePasswordDto>
{
    public AdminUserChangePasswordDtoValidator()
    {
        RuleFor(r => r.OldPassword).NotEmpty().WithName("كلمة المرور");
        RuleFor(r => r.NewPassword).Password("كلمة المرور الجديدة");
    }
}