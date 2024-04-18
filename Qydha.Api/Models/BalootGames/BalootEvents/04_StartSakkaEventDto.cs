namespace Qydha.API.Models;

[MapToEventName(nameof(StartSakkaEvent))]

public class StartSakkaEventDto : BalootGameEventDto
{
    public bool IsSakkaMashdoda { get; set; }

    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new StartSakkaEvent(IsSakkaMashdoda) { TriggeredAt = this.TriggeredAt };
    }
}
public class StartSakkaEventDtoValidator : AbstractValidator<StartSakkaEventDto>
{
    public StartSakkaEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(StartSakkaEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
