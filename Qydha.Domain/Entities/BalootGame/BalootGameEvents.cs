namespace Qydha.Domain.Entities;

public abstract class BalootGameEvent(string eventName)
{
    public readonly string EventName = eventName;
    public DateTimeOffset TriggeredAt { get; set; } = DateTimeOffset.UtcNow;
    public abstract Result ApplyToState(BalootGameState state);
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
    public override Result ApplyToState(BalootGameState state) => state.ChangeTeamsNames(UsName, ThemName);

}
public sealed class ChangeSakkaCountPerGameEvent : BalootGameEvent
{
    private ChangeSakkaCountPerGameEvent() : base(nameof(ChangeSakkaCountPerGameEvent)) { }
    public ChangeSakkaCountPerGameEvent(int count) : base(nameof(ChangeSakkaCountPerGameEvent))
    {
        SakkaPerGameCount = count;
    }
    public int SakkaPerGameCount { get; set; } = 3;

    public override Result ApplyToState(BalootGameState state) => state.ChangeSakkaCount(SakkaPerGameCount);

}
public sealed class ChangeIsSakkaMashdodaEvent : BalootGameEvent
{
    private ChangeIsSakkaMashdodaEvent() : base(nameof(ChangeIsSakkaMashdodaEvent)) { }
    public ChangeIsSakkaMashdodaEvent(bool isSakkaMashdoda) : base(nameof(ChangeIsSakkaMashdodaEvent))
    {
        IsSakkaMashdoda = isSakkaMashdoda;
    }
    public bool IsSakkaMashdoda { get; set; }
    public override Result ApplyToState(BalootGameState state) => state.ChangeIsSakkaMashdoda(IsSakkaMashdoda);
}
public sealed class AddMashare3ToLastMoshtaraEvent : BalootGameEvent
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }
    public AddMashare3Details? AdvancedDetails { get; set; }

    private AddMashare3ToLastMoshtaraEvent() : base(nameof(AddMashare3ToLastMoshtaraEvent)) { }
    public AddMashare3ToLastMoshtaraEvent(int usScore, int themScore) : base(nameof(AddMashare3ToLastMoshtaraEvent))
    {
        UsAbnat = usScore;
        ThemAbnat = themScore;
        RecordingMode = BalootRecordingMode.Regular;
        AdvancedDetails = null;
    }
    public AddMashare3ToLastMoshtaraEvent(MoshtaraType moshtaraType, (int, int) sra, (int, int) khamsen, (int, int) me2a, (int, int)? baloot, (int, int)? rob3ome2a, BalootGameTeam? selectedMoshtaraOwner)
        : base(nameof(AddMashare3ToLastMoshtaraEvent))
    {
        RecordingMode = BalootRecordingMode.Advanced;
        UsAbnat = 0;
        ThemAbnat = 0;
        switch (moshtaraType)
        {
            case MoshtaraType.Sun:
                if (rob3ome2a is null) throw new ArgumentNullException(nameof(rob3ome2a));
                AdvancedDetails = new AddMashare3Details(
                    MoshtaraType.Sun,
                    new Mashare3Sun(sra.Item1, khamsen.Item1, me2a.Item1, rob3ome2a.Value.Item1),
                    new Mashare3Sun(sra.Item2, khamsen.Item2, me2a.Item2, rob3ome2a.Value.Item2),
                    selectedMoshtaraOwner
                );
                break;
            case MoshtaraType.Hokm:
                if (baloot is null) throw new ArgumentNullException(nameof(baloot));
                AdvancedDetails = new AddMashare3Details(
                   MoshtaraType.Sun,
                   new Mashare3Hokm(baloot.Value.Item1, sra.Item1, khamsen.Item1, me2a.Item1),
                   new Mashare3Hokm(baloot.Value.Item2, sra.Item2, khamsen.Item2, me2a.Item2),
                   selectedMoshtaraOwner
               );
                break;
        }
    }
    public override Result ApplyToState(BalootGameState state)
    {
        if (RecordingMode == BalootRecordingMode.Advanced && AdvancedDetails is not null)
            return state.AddMashare3ToLastMoshtara(AdvancedDetails.UsMashare3, AdvancedDetails.ThemMashare3, AdvancedDetails.SelectedMoshtaraOwner);
        else
            return state.AddMashare3ToLastMoshtara(UsAbnat, ThemAbnat);
    }

    public class AddMashare3Details(MoshtaraType moshtara, Mashare3 usMashare3, Mashare3 themMashare3, BalootGameTeam? selectedMoshtaraOwner)
    {
        public MoshtaraType Moshtara { get; set; } = moshtara;
        public BalootGameTeam? SelectedMoshtaraOwner { get; set; } = selectedMoshtaraOwner;
        public Mashare3 UsMashare3 { get; set; } = usMashare3;
        public Mashare3 ThemMashare3 { get; set; } = themMashare3;
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

    public override Result ApplyToState(BalootGameState state) => state.StartGame(UsName, ThemName, SakkaCountPerGame);
}
public sealed class StartSakkaEvent : BalootGameEvent
{
    private StartSakkaEvent() : base(nameof(StartSakkaEvent)) { }
    public StartSakkaEvent(bool isSakkaMashdoda) : base(nameof(StartSakkaEvent))
    {
        IsSakkaMashdoda = isSakkaMashdoda;
    }
    public bool IsSakkaMashdoda { get; set; }

    public override Result ApplyToState(BalootGameState state) => state.StartSakka(IsSakkaMashdoda, TriggeredAt);
}
public sealed class StartMoshtaraEvent() : BalootGameEvent(nameof(StartMoshtaraEvent))
{
    public override Result ApplyToState(BalootGameState state) => state.StartMoshtara(TriggeredAt);
}
public sealed class EndMoshtaraEvent : BalootGameEvent
{
    private EndMoshtaraEvent() : base(nameof(EndMoshtaraEvent)) { }
    public EndMoshtaraEvent(MoshtaraData data) : base(nameof(EndMoshtaraEvent))
    {
        MoshtaraData = data;
    }
    public MoshtaraData MoshtaraData { get; set; } = null!;

    public override Result ApplyToState(BalootGameState state) =>
        state.EndMoshtara(MoshtaraData, TriggeredAt);
}
public sealed class RemoveMoshtaraEvent() : BalootGameEvent(nameof(RemoveMoshtaraEvent))
{
    public override Result ApplyToState(BalootGameState state) => state.Back();
}
public sealed class EndSakkaEvent : BalootGameEvent
{
    private EndSakkaEvent() : base(nameof(EndSakkaEvent)) { }
    public EndSakkaEvent(BalootGameTeam winnerTeam, BalootDrawHandler drawHandler = BalootDrawHandler.None) : base(nameof(EndSakkaEvent))
    {
        Winner = winnerTeam;
        DrawHandler = drawHandler;
    }
    public BalootGameTeam Winner { get; set; }
    public BalootDrawHandler DrawHandler { get; set; }
    public override Result ApplyToState(BalootGameState state) => state.EndSakka(Winner, DrawHandler, TriggeredAt);
}
public sealed class EndGameEvent : BalootGameEvent
{
    private EndGameEvent() : base(nameof(EndGameEvent)) { }
    public EndGameEvent(BalootGameTeam winnerTeam) : base(nameof(EndGameEvent))
    {
        Winner = winnerTeam;
    }
    public BalootGameTeam Winner { get; set; }
    public override Result ApplyToState(BalootGameState state) => state.EndGame(Winner);
}
public sealed class PauseGameEvent() : BalootGameEvent(nameof(PauseGameEvent))
{
    public override Result ApplyToState(BalootGameState state) => state.PauseGame(TriggeredAt);
}
public sealed class ResumeGameEvent() : BalootGameEvent(nameof(ResumeGameEvent))
{
    public override Result ApplyToState(BalootGameState state) => state.ResumeGame(TriggeredAt);
}
#endregion

#region  book & chat events
public sealed class OpenBalootBookEvent() : BalootGameEvent(nameof(OpenBalootBookEvent))
{
    public override Result ApplyToState(BalootGameState state)
    {
        // ! TODO count book opens 
        return Result.Ok();
    }
}
public sealed class CloseBalootBookEvent() : BalootGameEvent(nameof(CloseBalootBookEvent))
{
    public override Result ApplyToState(BalootGameState state)
    {
        // ! TODO count book opens 
        return Result.Ok();
    }
}
public sealed class OpenRefereeChatEvent() : BalootGameEvent(nameof(OpenRefereeChatEvent))
{
    public override Result ApplyToState(BalootGameState state)
    {
        // ! TODO count chat opens 
        return Result.Ok();
    }
}
public sealed class CloseRefereeChatEvent() : BalootGameEvent(nameof(CloseRefereeChatEvent))
{
    public override Result ApplyToState(BalootGameState state)
    {
        // ! TODO count chat opens 
        return Result.Ok();
    }
}
#endregion

