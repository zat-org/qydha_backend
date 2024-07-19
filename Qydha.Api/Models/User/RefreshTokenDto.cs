namespace Qydha.API.Models;
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = null!;
    public string JwtToken { get; set; } = null!;
}
public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(r => r.RefreshToken).NotEmpty();
        RuleFor(r => r.JwtToken).NotEmpty();
    }
}