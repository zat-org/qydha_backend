namespace Qydha.API.Models;

public class UpdatePopupImageDto
{
    public IFormFile File { get; set; } = null!;

}
public class UpdatePopupImageDtoValidator : AbstractValidator<UpdatePopupImageDto>
{
    private readonly NotificationImageSettings _settings;
    public UpdatePopupImageDtoValidator(IOptions<NotificationImageSettings> settings)
    {
        _settings = settings.Value;
        RuleFor(dto => dto.File)
        .Cascade(CascadeMode.Stop)
        .File("صورة النافذة المنبثقة", _settings);
    }
}