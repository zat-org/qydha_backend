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
        RuleFor(r => r.Name).Name("الاسم");
        RuleFor(r => r.BirthDate).BirthDate("تاريخ الميلاد");
    }
}