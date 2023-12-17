namespace Qydha.API.Models;

public class ConfirmForgetPasswordDto
{
    public string Code { get; set; } = null!;
    public Guid RequestId { get; set; }
    public string? FCM_Token { get; set; }

}

public class ConfirmForgetPasswordDtoValidator : AbstractValidator<ConfirmForgetPasswordDto>
{
    public ConfirmForgetPasswordDtoValidator()
    {
        RuleFor(r => r.Code).OTPCode("كود التحقق");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
        When(r => r.FCM_Token is not null, () =>
        {
            RuleFor(r => r.FCM_Token).FCM_Token("FCM_Token");
        });
    }
}