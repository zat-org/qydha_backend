using FluentValidation.Validators;

namespace Qydha.API.Models;

public class NotificationSendToUserDto
{
    public string Title { get; set; }= null!;
    public string Description { get; set; }= null!;
    public string Action_Path { get; set; }= null!;
    public NotificationActionType Action_Type { get; set; }
    public Guid UserId { get; set; }
}
public class NotificationSendToUserDtoValidator : AbstractValidator<NotificationSendToUserDto>
{
    public NotificationSendToUserDtoValidator()
    {
        RuleFor(r => r.Title).NotEmpty().MinimumLength(5).MaximumLength(255);
        RuleFor(r => r.Description).NotEmpty().MinimumLength(5).MaximumLength(512);
        RuleFor(r => r.Action_Path).NotEmpty().MaximumLength(350);
        RuleFor(r => r.Action_Type).NotEmpty().IsInEnum().WithMessage("Invalid Action Type"); ;
        RuleFor(r => r.UserId).GuidId("رقم المستخدم");
    }

}