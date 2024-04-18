namespace Qydha.API.Models;

[MapToEventName(nameof(CloseRefereeChatEvent))]
public class CloseRefereeChatEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new CloseRefereeChatEvent() { TriggeredAt = TriggeredAt };
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
