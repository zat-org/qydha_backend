namespace Qydha.API.Models;

[MapToEventName(nameof(OpenRefereeChatEvent))]
public class OpenRefereeChatEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new OpenRefereeChatEvent() { TriggeredAt = this.TriggeredAt };
    }
}

public class OpenRefereeChatEventDtoValidator : AbstractValidator<OpenRefereeChatEventDto>
{
    public OpenRefereeChatEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(OpenRefereeChatEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
