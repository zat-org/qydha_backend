namespace Qydha.API.Models;

[MapToEventName(nameof(CloseBalootBookEvent))]
public class CloseBalootBookEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new CloseBalootBookEvent() { TriggeredAt = this.TriggeredAt };
    }
}

public class CloseBalootBookEventDtoValidator : AbstractValidator<CloseBalootBookEventDto>
{
    public CloseBalootBookEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(CloseBalootBookEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
