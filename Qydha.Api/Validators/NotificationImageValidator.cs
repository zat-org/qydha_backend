namespace Qydha.API.Validators;

public class NotificationImageValidator : AbstractValidator<IFormFile>
{
    private readonly NotificationImageSettings _settings;
    public NotificationImageValidator(IOptions<NotificationImageSettings> settings)
    {
        _settings = settings.Value;
        RuleFor(file => file)
                   .NotEmpty()
                   .WithMessage("صورة الاشعار حقل مطلوب.")
                   .Must(file => file.Length > 0)
                   .WithMessage("صورة الاشعار لا يمكن ان تكون فارغة")
                   .Must(file => file.Length <= _settings.MaxBytes)
                   .WithMessage("صورة الاشعار لا يمكن ان تتعدى الـ  MB 30 بالحجم")
                   .Must(file => IsValidMIME(Path.GetExtension(file.FileName)))
                   .WithMessage("يرجي ارفاق صورة الاشعار.");
    }


    public bool IsValidMIME(string mime)
    {
        mime = mime.ToLower();
        return _settings.AcceptedFileTypes.Any(x => $".{x.ToLower()}".Equals(mime));
    }
}
