using NetTopologySuite.Geometries;

namespace Qydha.API.Models;
[MapToEventName(nameof(StartBalootGameEvent))]
public class StartBalootGameEventDto : BalootGameEventDto
{
    public int SakkaCountPerGame { get; set; }
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        var location = Longitude != null && Latitude != null ? new Point(Longitude.Value, Latitude.Value) { SRID = 4326 } : null;
        BalootGameEvent e = new StartBalootGameEvent(UsName, ThemName, SakkaCountPerGame, location) { TriggeredAt = TriggeredAt };
        return Result.Ok(e);
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

        RuleFor(e => e.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
            .When(e => e.Latitude is not null);

        RuleFor(e => e.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
            .When(e => e.Longitude is not null);

    }
}
