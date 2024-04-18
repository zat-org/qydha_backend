namespace Qydha.API.Models;
[MapToEventName(nameof(ChangeIsSakkaMashdodaEvent))]

public class ChangeIsSakkaMashdodaEventDto : BalootGameEventDto
{
    public bool IsSakkaMashdoda { get; set; }

    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new ChangeIsSakkaMashdodaEvent(IsSakkaMashdoda) { TriggeredAt = this.TriggeredAt };
    }
}
public class ChangeIsSakkaMashdodaEventDtoValidator : AbstractValidator<ChangeIsSakkaMashdodaEventDto>
{
    public ChangeIsSakkaMashdodaEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(ChangeIsSakkaMashdodaEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
