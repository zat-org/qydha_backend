namespace Qydha.API.Models;

[MapToEventName(nameof(AddMashare3ToLastMoshtaraEvent))]
public class AddMashare3ToLastMoshtaraEventDto : BalootGameEventDto
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }

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
    }
}


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
    }
}
