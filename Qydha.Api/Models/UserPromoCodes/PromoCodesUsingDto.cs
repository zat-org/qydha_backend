namespace Qydha.API.Models;

public class PromoCodesUsingDto
{
    public Guid PromoCodeId { get; set; }
}

public class PromoCodesUsingDtoValidator : AbstractValidator<PromoCodesUsingDto>
{
    public PromoCodesUsingDtoValidator()
    {
        RuleFor(dto => dto.PromoCodeId)
        .NotEmpty();

    }
}