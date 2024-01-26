namespace Qydha.API.Models;

public class InfluencerCodeCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public int MaxCodesPerUserInGroup { get; set; } = 1;
}

public class InfluencerCodeCategoryDtoValidator : AbstractValidator<InfluencerCodeCategoryDto>
{
    public InfluencerCodeCategoryDtoValidator()
    {
        RuleFor(dto => dto.CategoryName).NotEmpty().Length(2, 100);
        RuleFor(dto => dto.MaxCodesPerUserInGroup).NotEmpty().GreaterThanOrEqualTo(1);
    }
}