namespace Qydha.API.Models;

public class ChangeUserFCMTokenDto
{
    public string FCM_Token { get; set; } = null!;
}

public class ChangeUserFCMTokenDtoValidator : AbstractValidator<ChangeUserFCMTokenDto>
{
    public ChangeUserFCMTokenDtoValidator()
    {
        RuleFor(r => r.FCM_Token).FCM_Token("FCM_Token");
    }
}