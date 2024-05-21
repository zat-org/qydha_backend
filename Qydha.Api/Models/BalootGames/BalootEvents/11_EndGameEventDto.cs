namespace Qydha.API.Models;

[MapToEventName(nameof(EndGameEvent))]
public class EndGameEventDto : BalootGameEventDto
{
    public BalootGameTeam Winner { get; set; }

    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new EndGameEvent(Winner) { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}

public class EndGameEventDtoValidator : AbstractValidator<EndGameEventDto>
{
    public EndGameEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(EndGameEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
        RuleFor(e => e.Winner).NotNull().IsInEnum();
    }
}
