namespace Qydha.API.Models;

[MapToEventName(nameof(EndMoshtaraEvent))]
public class EndMoshtaraEventDto : BalootGameEventDto
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }
    public AdvancedDetailsDto? AdvancedDetails { get; set; }
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        if (RecordingMode == BalootRecordingMode.Regular)
        {
            BalootGameEvent e = new EndMoshtaraEvent(new MoshtaraData(UsAbnat, ThemAbnat)) { TriggeredAt = this.TriggeredAt };
            return Result.Ok(e);
        }
        else
        {
            var moshtaraType = AdvancedDetails!.Moshtara;
            var ekak = (AdvancedDetails!.UsData.Ekak, AdvancedDetails!.ThemData.Ekak);
            var aklat = (AdvancedDetails!.UsData.Aklat, AdvancedDetails!.ThemData.Aklat);
            var sra = (AdvancedDetails!.UsData.Sra, AdvancedDetails!.ThemData.Sra);
            var khamsen = (AdvancedDetails!.UsData.Khamsen, AdvancedDetails!.ThemData.Khamsen);
            var me2a = (AdvancedDetails!.UsData.Me2a, AdvancedDetails!.ThemData.Me2a);

            (int, int)? baloot = AdvancedDetails!.Moshtara == MoshtaraType.Hokm ?
                (AdvancedDetails!.UsData.Baloot!.Value, AdvancedDetails!.ThemData.Baloot!.Value) : null;

            (int, int)? rob3ome2a = AdvancedDetails!.Moshtara == MoshtaraType.Sun ?
                (AdvancedDetails!.UsData.Rob3ome2a!.Value, AdvancedDetails!.ThemData.Rob3ome2a!.Value) : null;

            (SunMoshtaraScoresId, SunMoshtaraScoresId)? sunId = AdvancedDetails!.Moshtara == MoshtaraType.Sun ?
                (AdvancedDetails!.UsData.SunScoreId!.Value, AdvancedDetails!.ThemData.SunScoreId!.Value) : null;

            (HokmMoshtaraScoresId, HokmMoshtaraScoresId)? hokmId = AdvancedDetails!.Moshtara == MoshtaraType.Hokm ?
                        (AdvancedDetails!.UsData.HokmScoreId!.Value, AdvancedDetails!.ThemData.HokmScoreId!.Value) : null;

            return MoshtaraDetails.CreateMoshtaraDetails(moshtaraType, sunId, hokmId, sra, khamsen, me2a, baloot, rob3ome2a, ekak, aklat, AdvancedDetails!.SelectedMoshtaraOwner)
                .OnSuccess((moshtaraDetails) =>
                {
                    BalootGameEvent e = new EndMoshtaraEvent(new MoshtaraData(moshtaraDetails)) { TriggeredAt = this.TriggeredAt };
                    return Result.Ok(e);
                });
        }
    }
}

public class EndMoshtaraEventDtoValidator : AbstractValidator<EndMoshtaraEventDto>
{
    public EndMoshtaraEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(EndMoshtaraEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();

        RuleFor(e => e.RecordingMode).IsInEnum();

        When(e => e.RecordingMode == BalootRecordingMode.Regular, () =>
        {
            RuleFor(e => e.UsAbnat).GreaterThanOrEqualTo(0);
            RuleFor(e => e.ThemAbnat).GreaterThanOrEqualTo(0);
            RuleFor(e => e.ThemAbnat).GreaterThan(0).When(e => e.UsAbnat == 0);
            RuleFor(e => e.UsAbnat).GreaterThan(0).When(e => e.ThemAbnat == 0);
            RuleFor(e => e.AdvancedDetails).Null();
        })
        .Otherwise(() =>
        {
            RuleFor(e => e.UsAbnat).Equal(0);
            RuleFor(e => e.ThemAbnat).Equal(0);
            RuleFor(e => e.AdvancedDetails).NotNull();

            RuleFor(e => e.AdvancedDetails!.Moshtara).IsInEnum();
            RuleFor(e => e.AdvancedDetails!.UsData).SetValidator(new TeamAdvancedDetailsDtoValidator());
            RuleFor(e => e.AdvancedDetails!.ThemData).SetValidator(new TeamAdvancedDetailsDtoValidator());

            When(e => e.AdvancedDetails!.Moshtara == MoshtaraType.Sun, () =>
            {
                RuleFor(e => e.AdvancedDetails!.UsData.SunScoreId).NotNull().IsInEnum();
                RuleFor(e => e.AdvancedDetails!.ThemData.SunScoreId).NotNull().IsInEnum();

                RuleFor(x => x).Custom((model, context) =>
                {
                    var usScoreId = model.AdvancedDetails!.UsData.SunScoreId!.Value;
                    var themScoreId = model.AdvancedDetails!.ThemData.SunScoreId!.Value;
                    if (!BalootConstants.SunRoundScores[usScoreId].GoesToIds.Any(id => id == themScoreId))
                        context.AddFailure("قيم ال score الرئيسية غير صحيحة");
                }).When(x => x.AdvancedDetails!.UsData.SunScoreId != null && x.AdvancedDetails!.ThemData.SunScoreId != null);

                RuleFor(e => e.AdvancedDetails!.UsData.HokmScoreId).Null();
                RuleFor(e => e.AdvancedDetails!.ThemData.HokmScoreId).Null();

                RuleFor(e => e.AdvancedDetails!.UsData.Rob3ome2a).NotNull();
                RuleFor(e => e.AdvancedDetails!.ThemData.Rob3ome2a).NotNull();
                RuleFor(e => e.AdvancedDetails!.UsData.Baloot).Null();
                RuleFor(e => e.AdvancedDetails!.ThemData.Baloot).Null();
            });

            When(e => e.AdvancedDetails!.Moshtara == MoshtaraType.Hokm, () =>
            {
                RuleFor(e => e.AdvancedDetails!.UsData.SunScoreId).Null();
                RuleFor(e => e.AdvancedDetails!.ThemData.SunScoreId).Null();
                RuleFor(e => e.AdvancedDetails!.UsData.HokmScoreId).NotNull().IsInEnum();
                RuleFor(e => e.AdvancedDetails!.ThemData.HokmScoreId).NotNull().IsInEnum();

                RuleFor(x => x).Custom((model, context) =>
                {
                    var usScoreId = model.AdvancedDetails!.UsData.HokmScoreId!.Value;
                    var themScoreId = model.AdvancedDetails!.ThemData.HokmScoreId!.Value;
                    if (!BalootConstants.HokmRoundScores[usScoreId].GoesToIds.Any(id => id == themScoreId))
                        context.AddFailure("قيم ال score الرئيسية غير صحيحة");
                }).When(x => x.AdvancedDetails!.UsData.HokmScoreId != null && x.AdvancedDetails!.ThemData.HokmScoreId != null);

                RuleFor(e => e.AdvancedDetails!.UsData.Rob3ome2a).Null();
                RuleFor(e => e.AdvancedDetails!.ThemData.Rob3ome2a).Null();
                RuleFor(e => e.AdvancedDetails!.UsData.Baloot).NotNull();
                RuleFor(e => e.AdvancedDetails!.ThemData.Baloot).NotNull();
            });

            RuleFor(x => x).Custom((model, context) =>
            {
                int ekakSummation = model.AdvancedDetails!.UsData.Ekak + model.AdvancedDetails!.ThemData.Ekak;
                if (ekakSummation != 4 && ekakSummation != 0)
                {
                    context.AddFailure("مجموع الاكك يجب ان يساوي 4");
                }
            });
            RuleFor(x => x).Custom((model, context) =>
            {
                int aklatSummation = model.AdvancedDetails!.UsData.Aklat + model.AdvancedDetails!.ThemData.Aklat;
                if (aklatSummation != 8 && aklatSummation != 0)
                {
                    context.AddFailure("مجموع الاكلات يجب ان يساوي 8");
                }
            });

        });
    }
}
public class AdvancedDetailsDto
{
    public MoshtaraType Moshtara { get; set; }
    public BalootGameTeam? SelectedMoshtaraOwner { get; set; }
    public TeamAdvancedDetailsDto UsData { get; set; } = null!;
    public TeamAdvancedDetailsDto ThemData { get; set; } = null!;
}
public class TeamAdvancedDetailsDto : TeamMashare3DataDto
{
    public SunMoshtaraScoresId? SunScoreId { get; set; }
    public HokmMoshtaraScoresId? HokmScoreId { get; set; }
    public int Ekak { get; set; }  // 0-4
    public int Aklat { get; set; } // 0-8

}
public class TeamAdvancedDetailsDtoValidator : AbstractValidator<TeamAdvancedDetailsDto>
{
    public TeamAdvancedDetailsDtoValidator()
    {
        RuleFor(d => d.Sra).GreaterThanOrEqualTo(0);
        RuleFor(d => d.Khamsen).GreaterThanOrEqualTo(0);
        RuleFor(d => d.Me2a).GreaterThanOrEqualTo(0);
        RuleFor(d => d.Ekak).GreaterThanOrEqualTo(0).LessThanOrEqualTo(4);
        RuleFor(d => d.Aklat).GreaterThanOrEqualTo(0).LessThanOrEqualTo(8);
        When(d => d.Rob3ome2a != null, () =>
        {
            RuleFor(d => d.Rob3ome2a).GreaterThanOrEqualTo(0);
        });
        When(d => d.Baloot != null, () =>
        {
            RuleFor(d => d.Baloot).GreaterThanOrEqualTo(0);
        });

        When(d => d.SunScoreId != null, () =>
        {
            RuleFor(d => d.SunScoreId).NotEmpty().IsInEnum();
        });

        When(d => d.HokmScoreId != null, () =>
        {
            RuleFor(d => d.HokmScoreId).NotEmpty().IsInEnum();
        });
    }
}
