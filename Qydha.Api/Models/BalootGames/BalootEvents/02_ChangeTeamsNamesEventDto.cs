namespace Qydha.API.Models;
[MapToEventName(nameof(ChangeTeamsNamesEvent))]
public class ChangeTeamsNamesEventDto : BalootGameEventDto
{
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new ChangeTeamsNamesEvent(UsName, ThemName) { TriggeredAt = this.TriggeredAt };
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

