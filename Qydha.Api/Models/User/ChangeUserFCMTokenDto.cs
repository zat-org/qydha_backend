namespace Qydha.API.Models;

public class ChangeUserFCMTokenDto
{
    public string FCMToken { get; set; } = null!;
}

public class ChangeUserFCMTokenDtoValidator : AbstractValidator<ChangeUserFCMTokenDto>
{
    public ChangeUserFCMTokenDtoValidator()
    {
        RuleFor(r => r.FCMToken).FCM_Token("FCM_Token");
    }
}