using Stateless;

namespace Qydha.Domain.Entities;

public class BalootSakkaState
{
    #region  Sakka state , triggers enums 
    public enum SakkaState
    {
        Created,
        Running,
        RunningWithoutMoshtaras,
        RunningWithMoshtaras,
        Paused,
        Ended
    }
    public enum SakkaTrigger
    {
        StartSakka,
        EndSakka,
        PauseSakka,
        ResumeSakka,
        Back,
        AddMashare3,
        StartMoshtara,
        UpdateMoshtara,
        EndMoshtara,
        ChangeIsSakkaMashdoda

    }
    #endregion

    #region  data 
    public SakkaState State => _stateMachine.State;
    private readonly StateMachine<SakkaState, SakkaTrigger> _stateMachine;
    [JsonIgnore]
    public bool IsRunningWithMoshtaras
    {
        get => _stateMachine.IsInState(SakkaState.RunningWithMoshtaras);
    }
    [JsonIgnore]
    public bool IsCreated
    {
        get => _stateMachine.IsInState(SakkaState.Created);
    }
    [JsonIgnore]
    public bool IsEnded
    {
        get => _stateMachine.IsInState(SakkaState.Ended);
    }
    [JsonIgnore]
    public bool IsRunningWithoutMoshtaras
    {
        get => _stateMachine.IsInState(SakkaState.RunningWithoutMoshtaras);
    }

    public BalootMoshtaraState CurrentMoshtara { get; set; } = new();
    public List<BalootMoshtaraState> Moshtaras { get; set; } = [];
    public bool IsMashdoda { get; set; }

    [JsonIgnore]
    public int WinningScore
    {
        get => IsMashdoda ? 100 : 152;
    }
    public BalootGameTeam? Winner { get; set; }
    public BalootDrawHandler DrawHandler { get; set; } = BalootDrawHandler.ExtraMoshtara;
    public BalootGameTeam? CheckWinner(BalootDrawHandler handler, BalootGameTeam selectedWinner)
    {
        if (UsScore >= WinningScore && UsScore > ThemScore)
            return BalootGameTeam.Us;
        else if (ThemScore >= WinningScore && ThemScore > UsScore)
            return BalootGameTeam.Them;
        else if (UsScore >= WinningScore && ThemScore >= WinningScore && UsScore == ThemScore)
            return handler == BalootDrawHandler.SelectMoshtaraOwner ? selectedWinner : null;
        else
            return null;
    }

    public int UsScore
    {
        get => Moshtaras.Aggregate(0, (totalScore, moshtara) => totalScore + moshtara.UsScore);
    }
    public int ThemScore
    {
        get => Moshtaras.Aggregate(0, (totalScore, moshtara) => totalScore + moshtara.ThemScore);
    }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; } = null;
    public List<(DateTimeOffset StartAt, DateTimeOffset? EndAt)> PausingIntervals { get; set; } = [];

    public TimeSpan SakkaInterval
    {
        get
        {
            if (!_stateMachine.IsInState(SakkaState.Ended) || EndedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }

    #endregion

    #region  ctor
    public BalootSakkaState()
    {
        _stateMachine = new StateMachine<SakkaState, SakkaTrigger>(SakkaState.Created);
        ConfigureStateMachine();
    }

    [JsonConstructor]
    private BalootSakkaState(string state)
    {
        var memberState = (SakkaState)Enum.Parse(typeof(SakkaState), state);
        _stateMachine = new StateMachine<SakkaState, SakkaTrigger>(memberState);
        ConfigureStateMachine();
    }
    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(SakkaState.Created)
            .Permit(SakkaTrigger.StartSakka, SakkaState.RunningWithoutMoshtaras)
            .PermitReentry(SakkaTrigger.ChangeIsSakkaMashdoda);

        _stateMachine.Configure(SakkaState.Running);

        _stateMachine.Configure(SakkaState.RunningWithoutMoshtaras)
            .SubstateOf(SakkaState.Running)
            .PermitReentryIf(SakkaTrigger.StartMoshtara,
                () => CheckWinner(BalootDrawHandler.ExtraMoshtara, BalootGameTeam.Us) == null)
            .PermitIf(SakkaTrigger.EndMoshtara, SakkaState.RunningWithMoshtaras,
                () => CheckWinner(BalootDrawHandler.ExtraMoshtara, BalootGameTeam.Us) == null)
            .Permit(SakkaTrigger.PauseSakka, SakkaState.Paused)
            .PermitReentry(SakkaTrigger.ChangeIsSakkaMashdoda);

        _stateMachine.Configure(SakkaState.RunningWithMoshtaras)
            .SubstateOf(SakkaState.Running)
            .PermitReentryIf(SakkaTrigger.StartMoshtara,
                () => CheckWinner(BalootDrawHandler.ExtraMoshtara, BalootGameTeam.Us) == null)
            .PermitReentryIf(SakkaTrigger.EndMoshtara,
                () => CheckWinner(BalootDrawHandler.ExtraMoshtara, BalootGameTeam.Us) == null)
            .PermitReentryIf(SakkaTrigger.Back, () => Moshtaras.Count > 0)
            .PermitIf(SakkaTrigger.Back, SakkaState.RunningWithoutMoshtaras, () => Moshtaras.Count == 0)
            .Permit(SakkaTrigger.EndSakka, SakkaState.Ended)
            .Permit(SakkaTrigger.PauseSakka, SakkaState.Paused)
            .PermitReentry(SakkaTrigger.ChangeIsSakkaMashdoda)
            .PermitReentry(SakkaTrigger.AddMashare3)
            .PermitReentry(SakkaTrigger.UpdateMoshtara);

        _stateMachine.Configure(SakkaState.Paused)
            .PermitIf(SakkaTrigger.ResumeSakka, SakkaState.RunningWithoutMoshtaras, () => Moshtaras.Count == 0)
            .PermitIf(SakkaTrigger.ResumeSakka, SakkaState.RunningWithMoshtaras, () => Moshtaras.Count > 0)
            .PermitReentry(SakkaTrigger.ChangeIsSakkaMashdoda);

        _stateMachine.Configure(SakkaState.Ended)
            .Permit(SakkaTrigger.Back, SakkaState.RunningWithMoshtaras);
    }
    #endregion

    #region event handler

    public Result CanFire(SakkaTrigger trigger)
    {
        if (!_stateMachine.CanFire(trigger))
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire {trigger} On Sakka Current State {_stateMachine.State}"));
        return Result.Ok();
    }
    public Result ChangeIsSakkaMashdoda(bool isMashdoda)
    {
        return CanFire(SakkaTrigger.ChangeIsSakkaMashdoda)
        .OnSuccess(() =>
        {
            IsMashdoda = isMashdoda;
            _stateMachine.Fire(SakkaTrigger.ChangeIsSakkaMashdoda);
        });
    }
    public Result StartSakka(bool isMashdoda, DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.StartSakka)
        .OnSuccess(() =>
        {
            StartedAt = triggeredAt;
            IsMashdoda = isMashdoda;
            _stateMachine.Fire(SakkaTrigger.StartSakka);
        });
    }
    public Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.EndSakka)
        .OnSuccess(() =>
        {
            var calculatedWinner = CheckWinner(drawHandler, winner);
            if (calculatedWinner is null)
                return Result.Fail(new InvalidBalootGameActionError($"Can't EndSakka with total UsScore : {UsScore} total ThemScore : {ThemScore} and draw handler : {drawHandler}"));
            EndedAt = triggeredAt;
            Winner = calculatedWinner;
            DrawHandler = drawHandler;
            _stateMachine.Fire(SakkaTrigger.EndSakka);
            return Result.Ok();
        });
    }
    public Result PauseSakka(DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.PauseSakka)
        .OnSuccess(() =>
        {
            PausingIntervals.Add((triggeredAt, null));
            CurrentMoshtara.PauseMoshtara(triggeredAt);
            _stateMachine.Fire(SakkaTrigger.PauseSakka);
        });
    }
    public Result ResumeSakka(DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.ResumeSakka)
        .OnSuccess(() =>
        {
            var pauseInterval = PausingIntervals.Last();
            PausingIntervals.RemoveAt(PausingIntervals.Count - 1);
            pauseInterval.EndAt = triggeredAt;
            PausingIntervals.Add(pauseInterval);
            CurrentMoshtara.ResumeMoshtara(triggeredAt);
            _stateMachine.Fire(SakkaTrigger.ResumeSakka);
        });
    }
    public Result StartMoshtara(DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.StartMoshtara)
        .OnSuccess(() =>
        {
            _stateMachine.Fire(SakkaTrigger.StartMoshtara);
            return CurrentMoshtara.StartMoshtara(triggeredAt);
        });
    }
    public Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.EndMoshtara)
        .OnSuccess(() => CurrentMoshtara.EndMoshtara(moshtaraData, triggeredAt))
        .OnSuccess(() =>
        {
            _stateMachine.Fire(SakkaTrigger.EndMoshtara);
            Moshtaras.Add(CurrentMoshtara);
            CurrentMoshtara = new();
        });
    }
    public Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        return CanFire(SakkaTrigger.UpdateMoshtara)
        .OnSuccess(() =>
        {
            var moshtara = Moshtaras.LastOrDefault();
            if (moshtara == null)
            {
                var err = new InvalidBalootGameActionError($"there is no moshtara to be updated");
                return Result.Fail(err);
            }
            return moshtara.UpdateMoshtara(moshtaraData, triggeredAt);
        })
        .OnSuccess(() => CurrentMoshtara = new())
        .OnSuccess(() => _stateMachine.Fire(SakkaTrigger.UpdateMoshtara));
    }
    public Result AddMashare3ToLastMoshtara(int usScore, int themScore)
    {
        return CanFire(SakkaTrigger.AddMashare3)
        .OnSuccess(() => Moshtaras.Last().AddMashare3(usScore, themScore))
        .OnSuccess(() => _stateMachine.Fire(SakkaTrigger.AddMashare3));
    }

    public Result Back(bool withRemoveLastMoshtara = true)
    {
        return CanFire(SakkaTrigger.Back)
        .OnSuccess(() =>
        {
            if (Moshtaras.Count == 0)
                throw new InvalidBalootGameEventException($"can't apply event back on sakka that has not any moshtaras , its state {State}");

            if (_stateMachine.IsInState(SakkaState.Ended))
            {
                Winner = null;
                DrawHandler = BalootDrawHandler.ExtraMoshtara;
                EndedAt = null;
                if (withRemoveLastMoshtara)
                {
                    CurrentMoshtara = Moshtaras.Last();
                    Moshtaras.Remove(CurrentMoshtara);
                    return CurrentMoshtara.Back();
                }
                else
                    return Result.Ok();
            }
            else
            {
                // current state must be running with moshtaras 
                CurrentMoshtara = Moshtaras.Last();
                Moshtaras.Remove(CurrentMoshtara);
                return CurrentMoshtara.Back();
            }
        })
        .OnSuccess(() => _stateMachine.Fire(SakkaTrigger.Back));
    }

    #endregion

    public BalootGameStatistics GetStatistics() =>
        Moshtaras.Aggregate(BalootGameStatistics.Zero(), (total, moshtara) => total + moshtara.GetStatistics());

}

