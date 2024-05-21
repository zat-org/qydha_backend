namespace Qydha.API.Models;

[MapToEventName(nameof(RemoveMoshtaraEvent))]
public class RemoveMoshtaraEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new RemoveMoshtaraEvent() { TriggeredAt = TriggeredAt };
        return Result.Ok(e);
    }
}
public class RemoveMoshtaraEventDtoValidator : AbstractValidator<RemoveMoshtaraEventDto>
{
    public RemoveMoshtaraEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(RemoveMoshtaraEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
