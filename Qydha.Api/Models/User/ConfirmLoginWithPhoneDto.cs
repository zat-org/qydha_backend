namespace Qydha.API.Models;

public class ConfirmLoginWithPhoneDto
{
    public string Code { get; set; } = null!;
    public Guid RequestId { get; set; }
}

public class ConfirmLoginWithPhoneDtoValidator : AbstractValidator<ConfirmLoginWithPhoneDto>
{
    public ConfirmLoginWithPhoneDtoValidator()
    {
        RuleFor(r => r.Code).OTPCode("كود التحقق");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
    }
}