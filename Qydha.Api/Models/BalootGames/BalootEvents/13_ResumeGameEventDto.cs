namespace Qydha.API.Models;

[MapToEventName(nameof(ResumeGameEvent))]
public class ResumeGameEventDto : BalootGameEventDto
{
    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new ResumeGameEvent() { TriggeredAt = this.TriggeredAt };
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
