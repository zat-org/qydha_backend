namespace Qydha.API.Models;

[MapToEventName(nameof(RemoveMoshtaraEvent))]
public class RemoveMoshtaraEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new RemoveMoshtaraEvent() { TriggeredAt = TriggeredAt };
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
