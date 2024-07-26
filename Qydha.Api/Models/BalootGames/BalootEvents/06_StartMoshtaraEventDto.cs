namespace Qydha.API.Models;

[MapToEventName(nameof(StartMoshtaraEvent))]

public class StartMoshtaraEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new StartMoshtaraEvent() { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}
public class StartMoshtaraEventDtoValidator : AbstractValidator<StartMoshtaraEventDto>
{
    public StartMoshtaraEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(StartMoshtaraEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
