using Stateless;

namespace Qydha.Domain.Entities;

public class BalootGameState
{
    #region state , triggers enums
    public enum GameStates
    {
        Created,
        Running,
        RunningWithSakkas,
        RunningWithoutSakkas,
        Paused,
        Ended,
    }
    public enum GameTriggers
    {
        ChangeTeamsNames,
        ChangeIsSakkaMashdoda,
        ChangeSakkaMaxCount,
        StartGame,
        EndGame,
        StartSakka,
        EndSakka,
        PauseGame,
        ResumeGame,
        Back,
        AddMashare3,
        StartMoshtara,
        EndMoshtara,
        UpdateMoshtara
    }
    #endregion

    #region Data
    public GameStates State => _stateMachine.State;
    private readonly StateMachine<GameStates, GameTriggers> _stateMachine;
    public string UsName { get; set; } = "لنا";
    public string ThemName { get; set; } = "لهم";
    public int MaxSakkaPerGame { get; set; } = 1;
    public BalootSakkaState CurrentSakka { get; set; } = new();
    public List<BalootSakkaState> Sakkas { get; set; } = [];
    public int UsGameScore
    {
        get => Sakkas.Aggregate(0, (totalScore, sakka) => totalScore + (sakka.Winner != null && sakka.Winner == BalootGameTeam.Us ? 1 : 0));
    }
    public int ThemGameScore
    {
        get => Sakkas.Aggregate(0, (totalScore, sakka) => totalScore + (sakka.Winner != null && sakka.Winner == BalootGameTeam.Them ? 1 : 0));
    }
    public TimeSpan GameInterval
    {
        get
        {
            if (!_stateMachine.IsInState(GameStates.Ended) || EndedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }
    public BalootGameTeam? Winner { get; set; } = null;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; } = null;
    public List<(DateTimeOffset StartAt, DateTimeOffset? EndAt)> PausingIntervals { get; set; } = [];

    #endregion

    #region ctor
    public BalootGameState()
    {
        _stateMachine = new StateMachine<GameStates, GameTriggers>(GameStates.Created);
        ConfigureStateMachine();
    }

    [JsonConstructor]
    private BalootGameState(string state)
    {
        var memberState = (GameStates)Enum.Parse(typeof(GameStates), state);
        _stateMachine = new StateMachine<GameStates, GameTriggers>(memberState);
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(GameStates.Created)
            .Permit(GameTriggers.StartGame, GameStates.RunningWithoutSakkas);

        _stateMachine.Configure(GameStates.Running);

        _stateMachine.Configure(GameStates.RunningWithoutSakkas)
            .SubstateOf(GameStates.Running)
            .PermitReentry(GameTriggers.ChangeSakkaMaxCount)
            .PermitReentry(GameTriggers.ChangeTeamsNames)
            .PermitReentry(GameTriggers.ChangeIsSakkaMashdoda)
            .PermitReentry(GameTriggers.StartSakka)
            .PermitReentry(GameTriggers.StartMoshtara)
            .PermitReentry(GameTriggers.EndMoshtara)
            .Permit(GameTriggers.PauseGame, GameStates.Paused)
            .Permit(GameTriggers.EndSakka, GameStates.RunningWithSakkas)
            .PermitReentryIf(GameTriggers.Back, () => CurrentSakka.IsRunningWithMoshtaras)
            .PermitReentryIf(GameTriggers.AddMashare3, () => CurrentSakka.IsRunningWithMoshtaras)
            .PermitReentryIf(GameTriggers.UpdateMoshtara, () => CurrentSakka.IsRunningWithMoshtaras);



        _stateMachine.Configure(GameStates.RunningWithSakkas)
            .SubstateOf(GameStates.Running)
            .PermitReentry(GameTriggers.ChangeSakkaMaxCount)
            .PermitReentry(GameTriggers.ChangeTeamsNames)
            .PermitReentry(GameTriggers.ChangeIsSakkaMashdoda)
            .PermitReentryIf(GameTriggers.StartSakka, () => CheckWinner() == null)
            .PermitReentry(GameTriggers.StartMoshtara)
            .PermitReentry(GameTriggers.EndMoshtara)
            .Permit(GameTriggers.PauseGame, GameStates.Paused)
            .PermitReentry(GameTriggers.EndSakka)
            .Permit(GameTriggers.EndGame, GameStates.Ended)
            .PermitIf(GameTriggers.Back, GameStates.RunningWithoutSakkas, () => Sakkas.Count == 0)
            .PermitReentryIf(GameTriggers.Back, () => Sakkas.Count > 0)
            .PermitReentryIf(GameTriggers.AddMashare3, () => CurrentSakka.IsRunningWithMoshtaras)
            .PermitReentryIf(GameTriggers.UpdateMoshtara, () => CurrentSakka.IsRunningWithMoshtaras);

        _stateMachine.Configure(GameStates.Ended)
            .PermitIf(GameTriggers.Back, GameStates.RunningWithoutSakkas, () => Sakkas.Count == 0)
            .PermitIf(GameTriggers.Back, GameStates.RunningWithSakkas, () => Sakkas.Count > 0);

        _stateMachine.Configure(GameStates.Paused)
            .PermitIf(GameTriggers.ResumeGame, GameStates.RunningWithoutSakkas, () => Sakkas.Count == 0)
            .PermitIf(GameTriggers.ResumeGame, GameStates.RunningWithSakkas, () => Sakkas.Count > 0);

    }
    #endregion

    #region utils
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, BalootConstants.balootEventsSerializationSettings);
    }

    public static BalootGameState FromJson(string jsonString)
    {
        return JsonConvert.DeserializeObject<BalootGameState>(jsonString, BalootConstants.balootEventsSerializationSettings)
        ?? throw new JsonSerializationException("can't parse the object to the target ");
    }

    #endregion

    #region TimeLine

    public List<BalootGameTimeLineBlock> GetGameTimelineForEditing()
    {
        List<BalootGameTimeLineBlock> blocks = [];
        TimeSpan triggerAfter = TimeSpan.Zero;
        List<TimeSpan> SakkasIntervals = [];
        int usGameScore = 0;
        int themGameScore = 0;
        for (int sakkaIndex = 0; sakkaIndex < Sakkas.Count; sakkaIndex++)
        {
            var sakka = Sakkas[sakkaIndex];
            int usSakkaScore = 0;
            int themSakkaScore = 0;
            List<int> usSakkaScores = [];
            List<int> themSakkaScores = [];
            for (int moshtaraIndex = 0; moshtaraIndex < sakka.Moshtaras.Count; moshtaraIndex++)
            {
                var currentMoshtara = sakka.Moshtaras[moshtaraIndex];
                triggerAfter += currentMoshtara.MoshtaraInterval;
                usSakkaScore += currentMoshtara.UsScore;
                themSakkaScore += currentMoshtara.ThemScore;
                usSakkaScores.Add(currentMoshtara.UsScore);
                themSakkaScores.Add(currentMoshtara.ThemScore);
                if (moshtaraIndex == sakka.Moshtaras.Count - 1)
                {
                    usGameScore += sakka.Winner != null && sakka.Winner == BalootGameTeam.Us ? 1 : 0;
                    themGameScore += sakka.Winner != null && sakka.Winner == BalootGameTeam.Them ? 1 : 0;
                }
                blocks.Add(new(
                    moshtaraIndex == sakka.Moshtaras.Count - 1 ?
                            TimeLineBlockType.ScoreWithWinner : TimeLineBlockType.ScoreWithoutWinner,
                    triggerAfter.TotalSeconds,
                    new(UsName, usGameScore, usSakkaScore, [.. usSakkaScores]),
                    new(ThemName, themGameScore, themSakkaScore, [.. themSakkaScores])
                ));
            }

            SakkasIntervals.Add(sakka.SakkaInterval);
            triggerAfter = SakkasIntervals.Aggregate(TimeSpan.Zero, (a, b) => a + b);
        }

        List<int> usCurrentSakkaScores = [];
        List<int> themCurrentSakkaScores = [];
        for (int moshtaraIndex = 0; moshtaraIndex < CurrentSakka.Moshtaras.Count; moshtaraIndex++)
        {
            var currentMoshtara = CurrentSakka.Moshtaras[moshtaraIndex];
            triggerAfter += currentMoshtara.MoshtaraInterval;
            usCurrentSakkaScores.Add(currentMoshtara.UsScore);
            themCurrentSakkaScores.Add(currentMoshtara.ThemScore);
            blocks.Add(new(
                TimeLineBlockType.ScoreWithoutWinner,
                triggerAfter.TotalSeconds,
                new(UsName, usGameScore, usCurrentSakkaScores.Sum(), [.. usCurrentSakkaScores]),
                new(ThemName, themGameScore, themCurrentSakkaScores.Sum(), [.. themCurrentSakkaScores])
            ));
        }
        return blocks;
    }

    #endregion

    #region  statistics  

    public BalootGameStatistics GetStatistics() =>
        CurrentSakka.GetStatistics() +
            Sakkas.Aggregate(BalootGameStatistics.Zero(), (total, sakka) => total + sakka.GetStatistics());


    #endregion

    #region events handlers 
    public Result CanFire(GameTriggers trigger)
    {
        if (!_stateMachine.CanFire(trigger))
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire {trigger} On Game Current State {_stateMachine.State}"));
        return Result.Ok();
    }
    public Result ChangeTeamsNames(string usName, string themName)
    {
        return CanFire(GameTriggers.ChangeTeamsNames)
        .OnSuccess(() =>
        {
            UsName = usName;
            ThemName = themName;
            _stateMachine.Fire(GameTriggers.ChangeTeamsNames);
        });
    }

    public Result ChangeSakkaCount(int newSakkaCount)
    {
        return CanFire(GameTriggers.ChangeSakkaMaxCount)
        .OnSuccess(() =>
        {
            if (newSakkaCount >= MaxSakkaPerGame || (newSakkaCount > Sakkas.Count && CheckWinner(newSakkaCount) == null))
            {
                MaxSakkaPerGame = newSakkaCount;
                _stateMachine.Fire(GameTriggers.ChangeSakkaMaxCount);
                return Result.Ok();
            }
            return Result.Fail(new InvalidBalootGameActionError("Invalid MaxSakkaCountPerGame => doesn't lead to a non winner state."));
        });
    }

    public Result ChangeIsSakkaMashdoda(bool isSakkaMashdoda)
    {
        return CanFire(GameTriggers.ChangeIsSakkaMashdoda)
        .OnSuccess(() => CurrentSakka.ChangeIsSakkaMashdoda(isSakkaMashdoda))
        .OnSuccess(() => _stateMachine.Fire(GameTriggers.ChangeIsSakkaMashdoda));
    }
    public Result StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.StartGame)
        .OnSuccess(() =>
        {
            UsName = usName;
            ThemName = themName;
            MaxSakkaPerGame = sakkaCount;
            StartedAt = triggeredAt;
            _stateMachine.Fire(GameTriggers.StartGame);
        });
    }
    public Result StartSakka(bool isMashdoda, DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.StartSakka)
        .OnSuccess(() => CurrentSakka.StartSakka(isMashdoda, triggeredAt))
        .OnSuccess(() => _stateMachine.Fire(GameTriggers.StartSakka));
    }
    public Result StartMoshtara(DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.StartMoshtara)
        .OnSuccess(() => CurrentSakka.StartMoshtara(triggeredAt))
        .OnSuccess(() => _stateMachine.Fire(GameTriggers.StartMoshtara));
    }
    public Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.EndMoshtara)
        .OnSuccess(() => CurrentSakka.EndMoshtara(moshtaraData, triggeredAt))
        .OnSuccess(() => _stateMachine.Fire(GameTriggers.EndMoshtara));
    }
    public Result UpdateMoshtara(int moshtaraIndex, MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.UpdateMoshtara)
        .OnSuccess(() => CurrentSakka.UpdateMoshtara(moshtaraIndex, moshtaraData, triggeredAt))
        .OnSuccess(() => _stateMachine.Fire(GameTriggers.UpdateMoshtara));
    }
    public Result EndSakka(BalootGameTeam winner, BalootDrawHandler handler, DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.EndSakka)
        .OnSuccess(() => CurrentSakka.EndSakka(winner, handler, triggeredAt))
        .OnSuccess(() =>
        {
            Sakkas.Add(CurrentSakka);
            CurrentSakka = new();
            _stateMachine.Fire(GameTriggers.EndSakka);
        });
    }

    public Result AddMashare3ToLastMoshtara(int usScore, int themScore)
    {
        return CanFire(GameTriggers.AddMashare3)
        .OnSuccess(() => CurrentSakka.AddMashare3ToLastMoshtara(usScore, themScore))
        .OnSuccess(() => _stateMachine.Fire(GameTriggers.AddMashare3));
    }
    // public Result AddMashare3ToLastMoshtara(Mashare3 usMashare3, Mashare3 themMashare3, BalootGameTeam? selectedMoshtaraOwner)
    // {
    //     return CanFire(GameTriggers.AddMashare3)
    //     .OnSuccess(() => CurrentSakka.AddMashare3ToLastMoshtara(usMashare3, themMashare3, selectedMoshtaraOwner))
    //     .OnSuccess(() => _stateMachine.Fire(GameTriggers.AddMashare3));
    // }
    public Result Back()
    {
        return CanFire(GameTriggers.Back)
        .OnSuccess(() =>
        {
            if (_stateMachine.IsInState(GameStates.RunningWithoutSakkas))
            {
                _stateMachine.Fire(GameTriggers.Back);
                return CurrentSakka.Back();
            }
            else if (_stateMachine.IsInState(GameStates.Ended))
            {
                Winner = null;
                EndedAt = null;
                CurrentSakka = Sakkas.Last();
                Sakkas.Remove(CurrentSakka);
                _stateMachine.Fire(GameTriggers.Back);
                return CurrentSakka.Back();
            }
            else if (_stateMachine.IsInState(GameStates.RunningWithSakkas))
            {
                if (CurrentSakka.IsRunningWithoutMoshtaras || CurrentSakka.IsCreated)
                {
                    CurrentSakka = Sakkas.Last();
                    Sakkas.Remove(CurrentSakka);
                    _stateMachine.Fire(GameTriggers.Back);
                }
                return CurrentSakka.Back();
            }
            else
                return Result.Fail(new InvalidBalootGameActionError($"Invalid Trigger :: back to apply on Game state :: {State}."));
        });
    }
    public BalootGameTeam? CheckWinner(int? newMaxSakkaPerGame = null)
    {
        int maxSakkaPerGame = newMaxSakkaPerGame ?? MaxSakkaPerGame;
        int winningScore = maxSakkaPerGame / 2 + 1;
        if (UsGameScore >= winningScore && UsGameScore > ThemGameScore)
            return BalootGameTeam.Us;
        else if (ThemGameScore >= winningScore && ThemGameScore > UsGameScore)
            return BalootGameTeam.Them;
        else
            return null;
    }
    public Result EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
    {
        var calculatedWinner = CheckWinner();
        if (calculatedWinner is null)
            return Result.Fail(new InvalidBalootGameActionError($"Can't EndGame with total UsScore : {UsGameScore} total ThemScore : {ThemGameScore}"));
        Winner = calculatedWinner;
        EndedAt = triggeredAt;
        _stateMachine.Fire(GameTriggers.EndGame);
        return Result.Ok();
    }
    public Result PauseGame(DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.PauseGame)
        .OnSuccess(() => CurrentSakka.PauseSakka(triggeredAt))
        .OnSuccess(() =>
        {
            PausingIntervals.Add((triggeredAt, null));
            _stateMachine.Fire(GameTriggers.PauseGame);
        });
    }
    public Result ResumeGame(DateTimeOffset triggeredAt)
    {
        return CanFire(GameTriggers.ResumeGame)
        .OnSuccess(() => CurrentSakka.ResumeSakka(triggeredAt))
        .OnSuccess(() =>
        {
            var pauseInterval = PausingIntervals.Last();
            PausingIntervals.RemoveAt(PausingIntervals.Count - 1);
            pauseInterval.EndAt = triggeredAt;
            PausingIntervals.Add(pauseInterval);
            _stateMachine.Fire(GameTriggers.ResumeGame);
        });
    }
    #endregion
}
