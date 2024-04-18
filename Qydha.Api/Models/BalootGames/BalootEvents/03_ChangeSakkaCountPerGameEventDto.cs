namespace Qydha.API.Models;

[MapToEventName(nameof(ChangeSakkaCountPerGameEvent))]

public class ChangeSakkaCountPerGameEventDto : BalootGameEventDto
{
    public int SakkaCountPerGame { get; set; }

    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new ChangeSakkaCountPerGameEvent(SakkaCountPerGame) { TriggeredAt = this.TriggeredAt };
    }
}
public class ChangeSakkaCountPerGameEventDtoValidator : AbstractValidator<ChangeSakkaCountPerGameEventDto>
{
    public ChangeSakkaCountPerGameEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(ChangeSakkaCountPerGameEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();

        RuleFor(e => e.SakkaCountPerGame)
            .Must(val => BalootConstants.SakkaCountPerGameValues.Contains(val))
            .WithMessage("يجب ان يكون عدد الصكات فرديا بين 1 و11");
    }
}
