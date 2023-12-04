namespace Qydha.API.Models;

public class LoginWithPhoneDto
{
    public string? Phone { get; set; }
}


public class LoginWithPhoneDtoValidator : AbstractValidator<LoginWithPhoneDto>
{

    public LoginWithPhoneDtoValidator()
    {
        RuleFor(dto => dto.Phone).Phone("رقم الجوال");
    }
}