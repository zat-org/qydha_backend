namespace Qydha.API.Models;

[MapToEventName(nameof(OpenRefereeChatEvent))]
public class OpenRefereeChatEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new OpenRefereeChatEvent() { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
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
