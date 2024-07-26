namespace Qydha.API.Models;
[MapToEventName(nameof(ChangeTeamsNamesEvent))]
public class ChangeTeamsNamesEventDto : BalootGameEventDto
{
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new ChangeTeamsNamesEvent(UsName, ThemName) { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}
public class ChangeTeamsNamesEventDtoValidator : AbstractValidator<ChangeTeamsNamesEventDto>
{
    public ChangeTeamsNamesEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(ChangeTeamsNamesEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
        RuleFor(e => e.UsName).NotEmpty();
        RuleFor(e => e.ThemName).NotEmpty();
    }
}

