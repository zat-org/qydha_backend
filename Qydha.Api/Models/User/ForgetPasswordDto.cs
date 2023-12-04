namespace Qydha.API.Models;

public class ForgetPasswordDto
{
    public string? Phone { get; set; }
}


public class ForgetPasswordDtoValidator : AbstractValidator<ForgetPasswordDto>
{

    public ForgetPasswordDtoValidator()
    {
        RuleFor(dto => dto.Phone).Phone("رقم الجوال");
    }
}