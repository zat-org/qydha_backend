namespace Qydha.API.Models;

[MapToEventName(nameof(StartMoshtaraEvent))]

public class StartMoshtaraEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new StartMoshtaraEvent() { TriggeredAt = this.TriggeredAt };
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
