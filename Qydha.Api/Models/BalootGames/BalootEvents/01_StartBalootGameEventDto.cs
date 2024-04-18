namespace Qydha.API.Models;
[MapToEventName(nameof(StartBalootGameEvent))]
public class StartBalootGameEventDto : BalootGameEventDto
{
    public int SakkaCountPerGame { get; set; }
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;

    public override BalootGameEvent MapToCorrespondingEvent()
    {
        return new StartBalootGameEvent(UsName, ThemName, SakkaCountPerGame) { TriggeredAt = this.TriggeredAt };
    }
}
public class StartBalootGameEventValidator : AbstractValidator<StartBalootGameEventDto>
{
    public StartBalootGameEventValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(StartBalootGameEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();

        RuleFor(e => e.UsName).NotEmpty();
        RuleFor(e => e.ThemName).NotEmpty();

        RuleFor(e => e.SakkaCountPerGame)
            .Must(val => BalootConstants.SakkaCountPerGameValues.Contains(val))
            .WithMessage("يجب ان يكون عدد الصكات فرديا بين 1 و11");
    }
}
