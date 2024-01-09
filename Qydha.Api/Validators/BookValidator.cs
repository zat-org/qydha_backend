namespace Qydha.API.Validators;

public class BookValidator : AbstractValidator<IFormFile>
{
    private readonly BookSettings _settings;
    public BookValidator(IOptions<BookSettings> options)
    {
        _settings = options.Value;
        RuleFor(file => file)
                   .NotEmpty()
                   .WithMessage("الكتاب حقل مطلوب.")
                   .Must(file => file.Length > 0)
                   .WithMessage("الكتاب لا يمكن ان تكون فارغة")
                   .Must(file => file.Length <= _settings.MaxBytes)
                   .WithMessage("الكتاب لا يمكن ان تتعدى الـ  MB 30 بالحجم")
                   .Must(file => IsValidMIME(Path.GetExtension(file.FileName)))
                   .WithMessage("يرجي ارفاق الكتاب.");
    }


    public bool IsValidMIME(string mime)
    {
        mime = mime.ToLower();
        return _settings.AcceptedFileTypes.Any(x => $".{x.ToLower()}".Equals(mime));
    }

}
