namespace Qydha.API.Models;

[MapToEventName(nameof(PauseGameEvent))]
public class PauseGameEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new PauseGameEvent() { TriggeredAt = this.TriggeredAt };
    }
}

public class PauseGameEventDtoValidator : AbstractValidator<PauseGameEventDto>
{
    public PauseGameEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(PauseGameEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
