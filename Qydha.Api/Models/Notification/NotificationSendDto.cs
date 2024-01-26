
namespace Qydha.API.Models;

public class NotificationSendDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ActionPath { get; set; } = null!;
    public NotificationActionType ActionType { get; set; }
    public IFormFile? PopUpImage { get; set; }

}

public class NotificationSendDtoValidator : AbstractValidator<NotificationSendDto>
{
    private readonly NotificationImageSettings _settings;
    private readonly IEnumerable<string> ScreensNames = ["/baloot-game", "/hand-game", "/team-settings", "/edit-profile", "/app-settings", "/delete-user", "/change-password", "/about-us", "/privacy-policy"];
    private readonly IEnumerable<string> TabsNames = ["profile", "store", "statistics", "books", "home"];
    public NotificationSendDtoValidator(IOptions<NotificationImageSettings> settings)
    {
        _settings = settings.Value;

        RuleFor(r => r.Title).NotEmpty().MinimumLength(5).MaximumLength(255);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(5).MaximumLength(512);
        RuleFor(r => r.ActionType).NotEmpty().IsInEnum().WithMessage("Invalid Action Type");

        When(r => (r.ActionType & NotificationActionType.PopUp) == NotificationActionType.PopUp, () =>
        {
            RuleFor(r => r.ActionType).Must(val => val != NotificationActionType.PopUp)
            .WithMessage("Invalid Action Type"); ;

            RuleFor(r => r.PopUpImage)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("صورة الاشعار حقل مطلوب")
            .Must(file => file is not null && file.Length > 0)
            .WithMessage("صورة الاشعار لا يمكن ان تكون فارغة")
            .Must(file => file is not null && file.Length <= _settings.MaxBytes)
            .WithMessage("صورة الاشعار لا يمكن ان تتعدى الـ  MB 30 بالحجم")
            .Must(file => file is not null && IsValidMIME(Path.GetExtension(file.FileName)))
            .WithMessage("يرجي ارفاق صورة الاشعار بامتداد مناسب");
        })
        .Otherwise(() =>
        {
            RuleFor(r => r.PopUpImage).Null();
        });

        When(r => (r.ActionType & NotificationActionType.NoAction) == NotificationActionType.NoAction, () =>
        {
            RuleFor(r => r.ActionPath).NotNull().Equal("_");
        });

        When(r => (r.ActionType & NotificationActionType.GoToURL) == NotificationActionType.GoToURL, () =>
        {
            RuleFor(r => r.ActionPath).NotNull().MaximumLength(350);
        });

        When(r => (r.ActionType & NotificationActionType.GoToScreen) == NotificationActionType.GoToScreen, () =>
        {
            RuleFor(r => r.ActionPath).Must((val) => ScreensNames.Contains(val)).WithMessage("Invalid screen name");
        });

        When(r => (r.ActionType & NotificationActionType.GoToTab) == NotificationActionType.GoToTab, () =>
        {
            RuleFor(r => r.ActionPath).Must((val) => TabsNames.Contains(val)).WithMessage("Invalid Tab name");
        });

    }

    private bool IsValidMIME(string mime)
    {
        mime = mime.ToLower();
        return _settings.AcceptedFileTypes.Any(x => $".{x.ToLower()}".Equals(mime));
    }


}