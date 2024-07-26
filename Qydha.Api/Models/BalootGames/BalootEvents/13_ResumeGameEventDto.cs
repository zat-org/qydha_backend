namespace Qydha.API.Models;

[MapToEventName(nameof(ResumeGameEvent))]
public class ResumeGameEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new ResumeGameEvent() { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}

public class ResumeGameEventDtoValidator : AbstractValidator<ResumeGameEventDto>
{
    public ResumeGameEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(ResumeGameEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
