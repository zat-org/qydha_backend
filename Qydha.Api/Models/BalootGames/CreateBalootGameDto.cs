namespace Qydha.API.Models;

public class CreateBalootGameDto
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<BalootGameEventDto> Events { get; set; } = null!;
}
public class CreateBalootGameDtoValidator : AbstractValidator<CreateBalootGameDto>
{
    public CreateBalootGameDtoValidator()
    {
        RuleFor(dto => dto.Events).SetValidator(new BalootGameAddEventsValidator());
    }
}