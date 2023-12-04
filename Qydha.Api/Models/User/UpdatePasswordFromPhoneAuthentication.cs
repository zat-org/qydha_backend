namespace Qydha.API.Models;

public class UpdatePasswordFromPhoneAuthentication
{
    public Guid RequestId { get; set; }

    public string NewPassword { get; set; } = null!;
}
public class UpdatePasswordFromPhoneAuthenticationValidator : AbstractValidator<UpdatePasswordFromPhoneAuthentication>
{
    public UpdatePasswordFromPhoneAuthenticationValidator()
    {
        RuleFor(r => r.NewPassword).Password("كلمة السر");
        RuleFor(r => r.RequestId).GuidId("رقم الطلب");
    }
}