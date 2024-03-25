namespace Qydha.API.Models;

public class AddInfluencerCodeDto
{
    public string Code { get; set; } = null!;
    public int? CategoryId { get; set; }
    public int NumberOfDays { get; set; }
    public DateTimeOffset? ExpireAt { get; set; }
    public int MaxInfluencedUsersCount { get; set; }


}
public class AddInfluencerCodeDtoValidator : AbstractValidator<AddInfluencerCodeDto>
{
    public AddInfluencerCodeDtoValidator()
    {
        RuleFor(dto => dto.Code)
        .NotEmpty()
        .Length(2, 15);

        When(dto => dto.CategoryId is not null, () =>
        {
            RuleFor(dto => dto.CategoryId).GreaterThan(0);
        });

        RuleFor(dto => dto.NumberOfDays)
        .NotEmpty()
        .Must(val => val > 0 && val < 1000);

        When(dto => dto.ExpireAt is not null, () =>
        {
            RuleFor(dto => dto.ExpireAt)
                   .NotEmpty()
                   .Must(val => val > DateTimeOffset.UtcNow);
        });

        RuleFor(dto => dto.MaxInfluencedUsersCount)
        .GreaterThanOrEqualTo(0);
    }
}