namespace Qydha.API.Models;

public class LoginWithQydhaDto
{
    public string Username { get; set; } = string.Empty;
}

public class LoginWithQydhaDtoValidator : AbstractValidator<LoginWithQydhaDto>
{
    public LoginWithQydhaDtoValidator()
    {
        RuleFor(d => d.Username).NotEmpty();
    }
}
