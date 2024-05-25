namespace Qydha.API.Models;

[MapToEventName(nameof(EndSakkaEvent))]


public class EndSakkaEventDto : BalootGameEventDto
{
    public BalootGameTeam Winner { get; set; }
    public BalootDrawHandler DrawHandler { get; set; } = BalootDrawHandler.ExtraMoshtara;

    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new EndSakkaEvent(Winner, DrawHandler) { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}

public class EndSakkaEventDtoValidator : AbstractValidator<EndSakkaEventDto>
{
    public EndSakkaEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(EndSakkaEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
        RuleFor(e => e.Winner).NotNull().IsInEnum();
        RuleFor(e => e.DrawHandler).NotNull().IsInEnum();
    }
}
