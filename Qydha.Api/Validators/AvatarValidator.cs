
namespace Qydha.Api.Validators;
public class AvatarValidator : AbstractValidator<IFormFile>
{
    private readonly AvatarSettings _avatarSettings;
    public AvatarValidator(IOptions<AvatarSettings> avatarOptions)
    {
        _avatarSettings = avatarOptions.Value;
        RuleFor(file => file)
                   .NotEmpty()
                   .WithMessage("صورة المستخدم حقل مطلوب.")
                   .Must(file => file.Length > 0)
                   .WithMessage("صورة المستخدم لا يمكن ان تكون فارغة")
                   .Must(file => file.Length <= _avatarSettings.MaxBytes)
                   .WithMessage("صورة المستخدم لا يمكن ان تتعدى الـ  MB 30 بالحجم")
                   .Must(file => IsValidMIME(Path.GetExtension(file.FileName)))
                   .WithMessage("يرجي ارفاق صورة.");
    }


    public bool IsValidMIME(string mime)
    {
        mime = mime.ToLower();
        return _avatarSettings.AcceptedFileTypes.Any(x => $".{x.ToLower()}".Equals(mime));
    }

}
