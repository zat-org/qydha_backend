namespace Qydha.API.Models;

[MapToEventName(nameof(UpdateMoshtaraEvent))]
public class UpdateMoshtaraEventDto : BalootGameEventDto
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }
    public AdvancedDetailsDto? AdvancedDetails { get; set; }
    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        if (RecordingMode == BalootRecordingMode.Advanced && AdvancedDetails != null)
        {
            var moshtaraType = AdvancedDetails.Moshtara;
            var ekak = (AdvancedDetails.UsData.Ekak, AdvancedDetails.ThemData.Ekak);
            var aklat = (AdvancedDetails.UsData.Aklat, AdvancedDetails.ThemData.Aklat);
            var sra = (AdvancedDetails.UsData.Sra, AdvancedDetails.ThemData.Sra);
            var khamsen = (AdvancedDetails.UsData.Khamsen, AdvancedDetails.ThemData.Khamsen);
            var me2a = (AdvancedDetails.UsData.Me2a, AdvancedDetails.ThemData.Me2a);

            (int, int)? baloot = AdvancedDetails.Moshtara == MoshtaraType.Hokm ?
                (AdvancedDetails.UsData.Baloot!.Value, AdvancedDetails.ThemData.Baloot!.Value) : null;

            (int, int)? rob3ome2a = AdvancedDetails.Moshtara == MoshtaraType.Sun ?
                (AdvancedDetails.UsData.Rob3ome2a!.Value, AdvancedDetails.ThemData.Rob3ome2a!.Value) : null;

            (SunMoshtaraScoresId, SunMoshtaraScoresId)? sunId = AdvancedDetails.Moshtara == MoshtaraType.Sun ?
                (AdvancedDetails.UsData.SunScoreId!.Value, AdvancedDetails.ThemData.SunScoreId!.Value) : null;

            (HokmMoshtaraScoresId, HokmMoshtaraScoresId)? hokmId = AdvancedDetails.Moshtara == MoshtaraType.Hokm ?
                        (AdvancedDetails.UsData.HokmScoreId!.Value, AdvancedDetails.ThemData.HokmScoreId!.Value) : null;

            return MoshtaraDetails.CreateMoshtaraDetails(moshtaraType, sunId, hokmId, sra, khamsen, me2a, baloot, rob3ome2a, ekak, aklat, AdvancedDetails.SelectedMoshtaraOwner)
                .OnSuccess((moshtaraDetails) =>
                {
                    BalootGameEvent e = new UpdateMoshtaraEvent(new MoshtaraData(moshtaraDetails)) { TriggeredAt = this.TriggeredAt };
                    return Result.Ok(e);
                });
        }
        else if (RecordingMode == BalootRecordingMode.Regular)
        {
            BalootGameEvent e = new UpdateMoshtaraEvent(new MoshtaraData(UsAbnat, ThemAbnat))
            { TriggeredAt = this.TriggeredAt };
            return Result.Ok(e);
        }
        else
            return Result.Fail(new InvalidBodyInputError("update event in advanced recording mode must be with advanced details not equal null."));
    }
}

public class UpdateMoshtaraEventDtoValidator : AbstractValidator<UpdateMoshtaraEventDto>
{
    public UpdateMoshtaraEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(UpdateMoshtaraEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
        RuleFor(e => e.RecordingMode).IsInEnum();

        When(e => e.RecordingMode == BalootRecordingMode.Regular, () =>
        {
            RuleFor(e => e.UsAbnat).GreaterThanOrEqualTo(0);
            RuleFor(e => e.ThemAbnat).GreaterThanOrEqualTo(0);
            RuleFor(e => e.ThemAbnat).GreaterThan(0).When(e => e.UsAbnat == 0);
            RuleFor(e => e.UsAbnat).GreaterThan(0).When(e => e.ThemAbnat == 0);
            RuleFor(e => e.AdvancedDetails).Null();
        });
        When(e => e.RecordingMode == BalootRecordingMode.Advanced, () =>
        {
            RuleFor(e => e.UsAbnat).Equal(0);
            RuleFor(e => e.ThemAbnat).Equal(0);
            RuleFor(e => e.AdvancedDetails).NotEmpty();
            RuleFor(e => e.AdvancedDetails!).SetValidator(new AdvancedDetailsDtoValidator());
        });
    }
}
