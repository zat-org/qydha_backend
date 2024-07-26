namespace Qydha.API.Models;

[MapToEventName(nameof(CloseRefereeChatEvent))]
public class CloseRefereeChatEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e =  new CloseRefereeChatEvent() { TriggeredAt = TriggeredAt };
        return Result.Ok(e);
    }
}

public class CloseRefereeChatEventDtoValidator : AbstractValidator<CloseRefereeChatEventDto>
{
    public CloseRefereeChatEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(CloseRefereeChatEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
