﻿namespace Qydha.API.Models;

[MapToEventName(nameof(PauseGameEvent))]
public class PauseGameEventDto : BalootGameEventDto
{
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        BalootGameEvent e = new PauseGameEvent() { TriggeredAt = this.TriggeredAt };
        return Result.Ok(e);
    }
}

public class PauseGameEventDtoValidator : AbstractValidator<PauseGameEventDto>
{
    public PauseGameEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(PauseGameEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
    }
}
