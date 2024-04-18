namespace Qydha.API.Models;

[MapToEventName(nameof(OpenBalootBookEvent))]

public class OpenBalootBookEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new OpenBalootBookEvent() { TriggeredAt = this.TriggeredAt };
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
