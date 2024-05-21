namespace Qydha.API.Models;

[MapToEventName(nameof(OpenBalootBookEvent))]

public class OpenBalootBookEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new OpenBalootBookEvent() { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}

public class OpenBalootBookEventDtoValidator : AbstractValidator<OpenBalootBookEventDto>
{
    public OpenBalootBookEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(OpenBalootBookEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
