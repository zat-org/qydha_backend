namespace Qydha.API.Models;

public class ConfirmLoginWithQydhaDto
{
    public Guid RequestId { get; set; }
    public string Otp { get; set; } = null!;
}
public class ConfirmLoginWithQydhaDtoValidator : AbstractValidator<ConfirmLoginWithQydhaDto>
{
    public ConfirmLoginWithQydhaDtoValidator()
    {
        RuleFor(d => d.RequestId).Must((val) => val != Guid.Empty).WithMessage("لا يجب ان يكون رقم تعريف الطلب فارغا");
        RuleFor(d => d.Otp).OTPCode("رمز المرور");
    }
}
