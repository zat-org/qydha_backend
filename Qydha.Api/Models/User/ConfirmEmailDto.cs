namespace Qydha.API.Models;


public class ConfirmEmailDto
{
    public string Code { get; set; } = null!;
    public Guid RequestId { get; set; }
}
public class ConfirmEmailDtoValidator : AbstractValidator<ConfirmEmailDto>
{
    public ConfirmEmailDtoValidator()
    {
        RuleFor(r => r.Code).OTPCode("رمز المرور");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
    }
}