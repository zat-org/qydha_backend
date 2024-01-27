namespace Qydha.API.Models;

public class UpdateUserAvatarDto
{
    public IFormFile File { get; set; } = null!;
}

public class UpdateUserAvatarDtoValidator : AbstractValidator<UpdateUserAvatarDto>
{
    private readonly AvatarSettings _avatarSettings;
    public UpdateUserAvatarDtoValidator(IOptions<AvatarSettings> avatarOptions)
    {
        _avatarSettings = avatarOptions.Value;
        RuleFor(dto => dto.File)
        .Cascade(CascadeMode.Stop)
        .File("صورة المستخدم الشخصية", _avatarSettings);
    }
}