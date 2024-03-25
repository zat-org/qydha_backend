namespace Qydha.API.Models;

public class PromoCodesAddingDto
{
    public string Code { get; set; } = null!;
    public Guid UserId { get; set; }
    public int NumberOfDays { get; set; }
    public DateTimeOffset ExpireAt { get; set; }

}
public class PromoCodesAddingDtoValidator : AbstractValidator<PromoCodesAddingDto>
{
    public PromoCodesAddingDtoValidator()
    {
        RuleFor(dto => dto.Code)
        .NotEmpty()
        .Length(2, 15);

        RuleFor(dto => dto.NumberOfDays)
        .NotEmpty()
        .Must(val => val > 0 && val < 1000);

        RuleFor(dto => dto.ExpireAt)
        .NotEmpty()
        .Must(val => val > DateTimeOffset.UtcNow);
    }
}