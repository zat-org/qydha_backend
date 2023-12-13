namespace Qydha.API.Models;


public class UseInfluencerCodeDto
{
    public string Code { get; set; } = null!;
}

public class UseInfluencerCodeDtoValidator : AbstractValidator<UseInfluencerCodeDto>
{
    public UseInfluencerCodeDtoValidator()
    {
        RuleFor(dto => dto.Code)
        .NotEmpty();
    }
}