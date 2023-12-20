namespace Qydha.API.Models;

public class ConfirmLoginWithPhoneDto
{
    public string Code { get; set; } = null!;
    public Guid RequestId { get; set; }
    public string? FCMToken { get; set; }

}

public class ConfirmLoginWithPhoneDtoValidator : AbstractValidator<ConfirmLoginWithPhoneDto>
{
    public ConfirmLoginWithPhoneDtoValidator()
    {
        RuleFor(r => r.Code).OTPCode("كود التحقق");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
        When(r => r.FCMToken is not null, () =>
       {
           RuleFor(r => r.FCMToken).FCM_Token("FCM_Token");
       });
    }
}