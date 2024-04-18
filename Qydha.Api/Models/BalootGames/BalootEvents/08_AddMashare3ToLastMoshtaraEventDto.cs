namespace Qydha.API.Models;

[MapToEventName(nameof(AddMashare3ToLastMoshtaraEvent))]
public class AddMashare3ToLastMoshtaraEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        //! ToDo add data for this event 
        return new AddMashare3ToLastMoshtaraEvent(true) { TriggeredAt = TriggeredAt };
    }
}
public class AddMashare3ToLastMoshtaraEventDtoValidator : AbstractValidator<AddMashare3ToLastMoshtaraEventDto>
{
    public AddMashare3ToLastMoshtaraEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(AddMashare3ToLastMoshtaraEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
