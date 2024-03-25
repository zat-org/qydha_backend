namespace Qydha.API.Models;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset? BirthDate { get; set; } = null;
}
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        When(r => !string.IsNullOrEmpty(r.Name), () =>
        {
            RuleFor(r => r.Name).Name("الاسم");
        });
        When(r => r.BirthDate is not null, () =>
        {
            RuleFor(r => r.BirthDate).BirthDate("تاريخ الميلاد");
        });
    }
}