namespace Qydha.API.Models;

public class UpdateUserDto
{
    public string Name { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
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