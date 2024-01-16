
namespace Qydha.API.Models;

public class AddInfluencerCodeDto
{
    public string Code { get; set; } = null!;
    public int NumberOfDays { get; set; }
    public DateTime? ExpireAt { get; set; }
    public int MaxInfluencedUsersCount { get; set; }


}
public class AddInfluencerCodeDtoValidator : AbstractValidator<AddInfluencerCodeDto>
{
    public AddInfluencerCodeDtoValidator()
    {
        RuleFor(dto => dto.Code)
        .NotEmpty()
        .Length(2, 15);

        RuleFor(dto => dto.NumberOfDays)
        .NotEmpty()
        .Must(val => val > 0 && val < 1000);

        When(dto => dto.ExpireAt is not null, () =>
        {
            RuleFor(dto => dto.ExpireAt)
                   .NotEmpty()
                   .Must(val => val > DateTime.UtcNow);
        });

        RuleFor(dto => dto.MaxInfluencedUsersCount)
        .NotEmpty()
        .GreaterThanOrEqualTo(0);
    }
}