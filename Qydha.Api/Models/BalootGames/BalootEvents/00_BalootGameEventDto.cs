namespace Qydha.API.Models;

public abstract class BalootGameEventDto
{
    public string EventName { get; set; } = null!;
    public DateTimeOffset TriggeredAt { get; set; }
    public abstract BalootGameEvent MapToCorrespondingEvent();
}
public class BalootGameAddEventsValidator : AbstractValidator<List<BalootGameEventDto>>
{
    public BalootGameAddEventsValidator()
    {
        RuleForEach(dto => dto).SetInheritanceValidator(v =>
        {
            v.Add(new StartBalootGameEventValidator());
            v.Add(new ChangeTeamsNamesEventDtoValidator());
            v.Add(new ChangeSakkaCountPerGameEventDtoValidator());

            v.Add(new StartSakkaEventDtoValidator());
            v.Add(new ChangeIsSakkaMashdodaEventDtoValidator());

            v.Add(new StartMoshtaraEventDtoValidator());
            v.Add(new EndMoshtaraEventDtoValidator());
            v.Add(new AddMashare3ToLastMoshtaraEventDtoValidator());
            v.Add(new RemoveMoshtaraEventDtoValidator());

            v.Add(new EndSakkaEventDtoValidator());
            v.Add(new EndGameEventDtoValidator());

            v.Add(new PauseGameEventDtoValidator());
            v.Add(new ResumeGameEventDtoValidator());

            v.Add(new OpenBalootBookEventDtoValidator());
            v.Add(new CloseBalootBookEventDtoValidator());

            v.Add(new OpenRefereeChatEventDtoValidator());
            v.Add(new CloseRefereeChatEventDtoValidator());
        });
    }
}

