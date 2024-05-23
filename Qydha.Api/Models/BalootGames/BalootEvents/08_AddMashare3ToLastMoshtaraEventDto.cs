namespace Qydha.API.Models;

[MapToEventName(nameof(AddMashare3ToLastMoshtaraEvent))]
public class AddMashare3ToLastMoshtaraEventDto : BalootGameEventDto
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }

    // public AdvancedMashare3DetailsDto? AdvancedDetails { get; set; }
    // public BalootGameTeam? SelectedMoshtaraOwner { get; set; }

    public override Result<BalootGameEvent> MapToCorrespondingEvent()
    {
        if (RecordingMode != BalootRecordingMode.Regular)
        {
            var err = new InvalidBodyInputError("Mashare3 can only be added during regular Recoding");
            err.ValidationErrors.Add(nameof(RecordingMode), ["لا يمكن اضافة مشاريع الا في حالة التسجيل العادى"]);
            return Result.Fail(err);
        }
        BalootGameEvent regularAddMashare3 = new AddMashare3ToLastMoshtaraEvent(UsAbnat, ThemAbnat) { TriggeredAt = TriggeredAt };
        return Result.Ok(regularAddMashare3);
        // switch (RecordingMode)
        // {
        //     case BalootRecordingMode.Regular:
        //         BalootGameEvent regularAddMashare3 = new AddMashare3ToLastMoshtaraEvent(UsAbnat, ThemAbnat) { TriggeredAt = TriggeredAt };
        //         return Result.Ok(regularAddMashare3);
        //     case BalootRecordingMode.Advanced:
        //         var moshtaraType = AdvancedDetails!.Moshtara;
        //         var sra = (AdvancedDetails!.UsData.Sra, AdvancedDetails!.ThemData.Sra);
        //         var khamsen = (AdvancedDetails!.UsData.Khamsen, AdvancedDetails!.ThemData.Khamsen);
        //         var me2a = (AdvancedDetails!.UsData.Me2a, AdvancedDetails!.ThemData.Me2a);

        //         (int, int)? baloot = AdvancedDetails!.Moshtara == MoshtaraType.Hokm ?
        //             (AdvancedDetails!.UsData.Baloot!.Value, AdvancedDetails!.ThemData.Baloot!.Value) : null;

        //         (int, int)? rob3ome2a = AdvancedDetails!.Moshtara == MoshtaraType.Sun ?
        //             (AdvancedDetails!.UsData.Rob3ome2a!.Value, AdvancedDetails!.ThemData.Rob3ome2a!.Value) : null;

        //         BalootGameEvent advancedAddMashare3 = new AddMashare3ToLastMoshtaraEvent(moshtaraType, sra, khamsen, me2a, baloot, rob3ome2a, SelectedMoshtaraOwner)
        //         { TriggeredAt = TriggeredAt };
        //         return Result.Ok(advancedAddMashare3);
        //     default:
        //         throw new InvalidOperationException();
        //}
    }
}
// public class AdvancedMashare3DetailsDto
// {
//     public MoshtaraType Moshtara { get; set; }
//     public TeamMashare3DataDto UsData { get; set; } = null!;
//     public TeamMashare3DataDto ThemData { get; set; } = null!;
// }

public class AddMashare3ToLastMoshtaraEventDtoValidator : AbstractValidator<AddMashare3ToLastMoshtaraEventDto>
{
    public AddMashare3ToLastMoshtaraEventDtoValidator()
    {
        RuleFor(e => e.EventName).NotEmpty().Equal(nameof(AddMashare3ToLastMoshtaraEvent));
        RuleFor(e => e.TriggeredAt).NotEmpty();
        RuleFor(e => e.RecordingMode).IsInEnum().Equals(BalootRecordingMode.Regular);

        RuleFor(e => e.UsAbnat).GreaterThanOrEqualTo(0);
        RuleFor(e => e.ThemAbnat).GreaterThanOrEqualTo(0);
        RuleFor(e => e.ThemAbnat).GreaterThan(0).When(e => e.UsAbnat == 0);
        RuleFor(e => e.UsAbnat).GreaterThan(0).When(e => e.ThemAbnat == 0);

        // When(e => e.AdvancedDetails is null, () =>
        // {
        // RuleFor(e => e.UsAbnat).GreaterThanOrEqualTo(0);
        // RuleFor(e => e.ThemAbnat).GreaterThanOrEqualTo(0);
        // RuleFor(e => e.ThemAbnat).GreaterThan(0).When(e => e.UsAbnat == 0);
        // RuleFor(e => e.UsAbnat).GreaterThan(0).When(e => e.ThemAbnat == 0);
        // })
        // .Otherwise(() =>
        // {
        //     RuleFor(e => e.UsAbnat).Equal(0);
        //     RuleFor(e => e.ThemAbnat).Equal(0);

        //     RuleFor(e => e.AdvancedDetails!.Moshtara).IsInEnum();
        //     RuleFor(e => e.AdvancedDetails!.UsData).SetValidator(new TeamMashare3DataDtoValidator());
        //     RuleFor(e => e.AdvancedDetails!.ThemData).SetValidator(new TeamMashare3DataDtoValidator());

        //     When(e => e.AdvancedDetails!.Moshtara == MoshtaraType.Sun, () =>
        //     {
        //         RuleFor(e => e.AdvancedDetails!.UsData.Rob3ome2a).NotNull();
        //         RuleFor(e => e.AdvancedDetails!.ThemData.Rob3ome2a).NotNull();
        //         RuleFor(e => e.AdvancedDetails!.UsData.Baloot).Null();
        //         RuleFor(e => e.AdvancedDetails!.ThemData.Baloot).Null();
        //     });

        //     When(e => e.AdvancedDetails!.Moshtara == MoshtaraType.Hokm, () =>
        //     {
        //         RuleFor(e => e.AdvancedDetails!.UsData.Rob3ome2a).Null();
        //         RuleFor(e => e.AdvancedDetails!.ThemData.Rob3ome2a).Null();
        //         RuleFor(e => e.AdvancedDetails!.UsData.Baloot).NotNull();
        //         RuleFor(e => e.AdvancedDetails!.ThemData.Baloot).NotNull();
        //     });
        // });
    }

}

// public class TeamMashare3DataDtoValidator : AbstractValidator<TeamMashare3DataDto>
// {
//     public TeamMashare3DataDtoValidator()
//     {
//         RuleFor(d => d.Sra).GreaterThanOrEqualTo(0);
//         RuleFor(d => d.Khamsen).GreaterThanOrEqualTo(0);
//         RuleFor(d => d.Me2a).GreaterThanOrEqualTo(0);
//         When(d => d.Rob3ome2a != null, () =>
//         {
//             RuleFor(d => d.Rob3ome2a).GreaterThanOrEqualTo(0);
//         });
//         When(d => d.Baloot != null, () =>
//         {
//             RuleFor(d => d.Baloot).GreaterThanOrEqualTo(0);
//         });
//     }
// }