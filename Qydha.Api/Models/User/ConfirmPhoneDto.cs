namespace Qydha.API.Models;


public class ConfirmPhoneDto
{
    public string Code { get; set; } = null!;
    public Guid RequestId { get; set; }
}
public class ConfirmPhoneDtoValidator : AbstractValidator<ConfirmPhoneDto>
{
    public ConfirmPhoneDtoValidator()
    {
        RuleFor(r => r.Code).OTPCode("رمز المرور");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
       
    }
}