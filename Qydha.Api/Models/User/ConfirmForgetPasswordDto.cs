namespace Qydha.API.Models;

public class ConfirmForgetPasswordDto
{
    public string Code { get; set; } = null!;
    public Guid RequestId { get; set; }
}

public class ConfirmForgetPasswordDtoValidator : AbstractValidator<ConfirmForgetPasswordDto>
{
    public ConfirmForgetPasswordDtoValidator()
    {
        RuleFor(r => r.Code).OTPCode("كود التحقق");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
    }
}