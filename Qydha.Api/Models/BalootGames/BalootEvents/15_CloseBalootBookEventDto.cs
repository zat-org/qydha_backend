namespace Qydha.API.Models;

[MapToEventName(nameof(CloseBalootBookEvent))]
public class CloseBalootBookEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new CloseBalootBookEvent() { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
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
