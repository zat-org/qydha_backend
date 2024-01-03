namespace Qydha.API.Models;

public class PopupDto
{
    public string ActionPath { get; set; } = null!;
    public PopupActionType ActionType { get; set; }
    public bool Show { get; set; }
}
public class PopupDtoValidator : AbstractValidator<PopupDto>
{
    public PopupDtoValidator()
    {
        RuleFor(r => r.ActionPath).NotEmpty().MaximumLength(350);
        RuleFor(r => r.ActionType).NotEmpty().IsInEnum().WithMessage("Invalid Action Type"); ;
    }
}