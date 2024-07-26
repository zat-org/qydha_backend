namespace Qydha.API.Models;

[MapToEventName(nameof(StartSakkaEvent))]

public class StartSakkaEventDto : BalootGameEventDto
{
    public bool IsSakkaMashdoda { get; set; }

    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new StartSakkaEvent(IsSakkaMashdoda) { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
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
