namespace Qydha.API.Models;


public class DeleteUserDto
{
    public string Password { get; set; }= null!;
}

public class DeleteUserDtoValidator : AbstractValidator<DeleteUserDto>
{
    public DeleteUserDtoValidator()
    {
        RuleFor(r => r.Password).Password("كلمة المرور");
    }
}