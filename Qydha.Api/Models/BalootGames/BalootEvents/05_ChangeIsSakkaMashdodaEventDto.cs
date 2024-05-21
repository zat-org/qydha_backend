namespace Qydha.API.Models;
[MapToEventName(nameof(ChangeIsSakkaMashdodaEvent))]

public class ChangeIsSakkaMashdodaEventDto : BalootGameEventDto
{
    public bool IsSakkaMashdoda { get; set; }

    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new ChangeIsSakkaMashdodaEvent(IsSakkaMashdoda) { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
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
