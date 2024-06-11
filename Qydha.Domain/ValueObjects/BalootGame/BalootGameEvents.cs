namespace Qydha.Domain.Entities;

public abstract class BalootGameEvent(string eventName)
{
    public readonly string EventName = eventName;
    public DateTimeOffset TriggeredAt { get; set; } = DateTimeOffset.UtcNow;
    public abstract Result ApplyToState(BalootGameData state);
}


#region change data event
public sealed class ChangeTeamsNamesEvent : BalootGameEvent
{
    private ChangeTeamsNamesEvent() : base(nameof(ChangeTeamsNamesEvent)) { }
    public ChangeTeamsNamesEvent(string usName, string themName) : base(nameof(ChangeTeamsNamesEvent))
    {
        UsName = usName;
        ThemName = themName;
    }
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;
    public override Result ApplyToState(BalootGameData state) => state.ChangeTeamsNames(UsName, ThemName);

}
public sealed class ChangeSakkaCountPerGameEvent : BalootGameEvent
{
    private ChangeSakkaCountPerGameEvent() : base(nameof(ChangeSakkaCountPerGameEvent)) { }
    public ChangeSakkaCountPerGameEvent(int count) : base(nameof(ChangeSakkaCountPerGameEvent))
    {
        SakkaPerGameCount = count;
    }
    public int SakkaPerGameCount { get; set; } = 3;

    public override Result ApplyToState(BalootGameData state) => state.ChangeSakkaCount(SakkaPerGameCount);

}
public sealed class ChangeIsSakkaMashdodaEvent : BalootGameEvent
{
    private ChangeIsSakkaMashdodaEvent() : base(nameof(ChangeIsSakkaMashdodaEvent)) { }
    public ChangeIsSakkaMashdodaEvent(bool isSakkaMashdoda) : base(nameof(ChangeIsSakkaMashdodaEvent))
    {
        IsSakkaMashdoda = isSakkaMashdoda;
    }
    public bool IsSakkaMashdoda { get; set; }
    public override Result ApplyToState(BalootGameData state) => state.ChangeIsCurrentSakkaMashdoda(IsSakkaMashdoda);
}
public sealed class AddMashare3ToLastMoshtaraEvent : BalootGameEvent
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }

    private AddMashare3ToLastMoshtaraEvent() : base(nameof(AddMashare3ToLastMoshtaraEvent)) { }
    public AddMashare3ToLastMoshtaraEvent(int usScore, int themScore) : base(nameof(AddMashare3ToLastMoshtaraEvent))
    {
        UsAbnat = usScore;
        ThemAbnat = themScore;
        RecordingMode = BalootRecordingMode.Regular;
    }

    public override Result ApplyToState(BalootGameData state)
    {
        if (RecordingMode != BalootRecordingMode.Regular)
            return Result.Fail(new InvalidBalootGameActionError("Mashare3 can only be added during regular Recoding"));

        return state.AddMashare3(UsAbnat, ThemAbnat);
    }


}

#endregion

#region  game flow events
public sealed class StartBalootGameEvent : BalootGameEvent
{
    private StartBalootGameEvent() : base(nameof(StartBalootGameEvent)) { }
    public StartBalootGameEvent(string usName, string themName, int sakkaCount) : base(nameof(StartBalootGameEvent))
    {
        UsName = usName;
        ThemName = themName;
        SakkaCountPerGame = sakkaCount;
    }
    public int SakkaCountPerGame { get; set; }
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;

    public override Result ApplyToState(BalootGameData state) => state.StartGame(UsName, ThemName, SakkaCountPerGame, TriggeredAt);
}
public sealed class StartSakkaEvent : BalootGameEvent
{
    private StartSakkaEvent() : base(nameof(StartSakkaEvent)) { }
    public StartSakkaEvent(bool isSakkaMashdoda) : base(nameof(StartSakkaEvent))
    {
        IsSakkaMashdoda = isSakkaMashdoda;
    }
    public bool IsSakkaMashdoda { get; set; }

    public override Result ApplyToState(BalootGameData state) => state.StartSakka(IsSakkaMashdoda, TriggeredAt);
}
public sealed class StartMoshtaraEvent() : BalootGameEvent(nameof(StartMoshtaraEvent))
{
    public override Result ApplyToState(BalootGameData state) => state.StartMoshtara(TriggeredAt);
}
public sealed class EndMoshtaraEvent : BalootGameEvent
{
    private EndMoshtaraEvent() : base(nameof(EndMoshtaraEvent)) { }
    public EndMoshtaraEvent(MoshtaraData data) : base(nameof(EndMoshtaraEvent))
    {
        MoshtaraData = data;
    }
    public MoshtaraData MoshtaraData { get; set; } = null!;

    public override Result ApplyToState(BalootGameData state) =>
        state.EndMoshtara(MoshtaraData, TriggeredAt);
}
public sealed class UpdateMoshtaraEvent : BalootGameEvent
{
    private UpdateMoshtaraEvent() : base(nameof(UpdateMoshtaraEvent)) { }
    public UpdateMoshtaraEvent(MoshtaraData data) : base(nameof(UpdateMoshtaraEvent))
    {
        MoshtaraData = data;
    }
    public MoshtaraData MoshtaraData { get; set; } = null!;

    public override Result ApplyToState(BalootGameData state) =>
        state.UpdateMoshtara(MoshtaraData, TriggeredAt);
}
public sealed class RemoveMoshtaraEvent() : BalootGameEvent(nameof(RemoveMoshtaraEvent))
{
    public override Result ApplyToState(BalootGameData state) => state.Back();
}
public sealed class EndSakkaEvent : BalootGameEvent
{
    private EndSakkaEvent() : base(nameof(EndSakkaEvent)) { }
    public EndSakkaEvent(BalootGameTeam winnerTeam, BalootDrawHandler drawHandler = BalootDrawHandler.ExtraMoshtara) : base(nameof(EndSakkaEvent))
    {
        Winner = winnerTeam;
        DrawHandler = drawHandler;
    }
    public BalootGameTeam Winner { get; set; }
    public BalootDrawHandler DrawHandler { get; set; } = BalootDrawHandler.ExtraMoshtara;
    public override Result ApplyToState(BalootGameData state) => state.EndSakka(Winner, DrawHandler, TriggeredAt);
}
public sealed class EndGameEvent : BalootGameEvent
{
    private EndGameEvent() : base(nameof(EndGameEvent)) { }
    public EndGameEvent(BalootGameTeam winnerTeam) : base(nameof(EndGameEvent))
    {
        Winner = winnerTeam;
    }
    public BalootGameTeam Winner { get; set; }
    public override Result ApplyToState(BalootGameData state) => state.EndGame(Winner, TriggeredAt);
}
public sealed class PauseGameEvent() : BalootGameEvent(nameof(PauseGameEvent))
{
    public override Result ApplyToState(BalootGameData state) => state.Pause(TriggeredAt);
}
public sealed class ResumeGameEvent() : BalootGameEvent(nameof(ResumeGameEvent))
{
    public override Result ApplyToState(BalootGameData state) => state.Resume(TriggeredAt);
}
#endregion

#region  book & chat events
public sealed class OpenBalootBookEvent() : BalootGameEvent(nameof(OpenBalootBookEvent))
{
    public override Result ApplyToState(BalootGameData state)
    {
        // ! TODO count book opens 
        return Result.Ok();
    }
}
public sealed class CloseBalootBookEvent() : BalootGameEvent(nameof(CloseBalootBookEvent))
{
    public override Result ApplyToState(BalootGameData state)
    {
        // ! TODO count book opens 
        return Result.Ok();
    }
}
public sealed class OpenRefereeChatEvent() : BalootGameEvent(nameof(OpenRefereeChatEvent))
{
    public override Result ApplyToState(BalootGameData state)
    {
        // ! TODO count chat opens 
        return Result.Ok();
    }
}
public sealed class CloseRefereeChatEvent() : BalootGameEvent(nameof(CloseRefereeChatEvent))
{
    public override Result ApplyToState(BalootGameData state)
    {
        // ! TODO count chat opens 
        return Result.Ok();
    }
}
#endregion

