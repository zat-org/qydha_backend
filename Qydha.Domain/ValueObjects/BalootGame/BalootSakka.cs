namespace Qydha.Domain.ValueObjects;
public class BalootSakka
{
    public BalootSakka()
    {
        State = new BalootSakkaCreatedState(this);
        CurrentMoshtara = new();
        Moshtaras = [];
        PausingIntervals = [];
    }
    [JsonConstructor]
    public BalootSakka(BalootSakkaStateEnum stateName, BalootMoshtara currentMoshtara, List<BalootMoshtara> moshtaras, bool isMashdoda, BalootGameTeam? winner, BalootDrawHandler drawHandler, DateTimeOffset? startedAt, DateTimeOffset? endedAt, List<PausingInterval> pausingIntervals)
    {
        CurrentMoshtara = currentMoshtara;
        Moshtaras = moshtaras;
        PausingIntervals = [];

        switch (stateName)
        {
            case BalootSakkaStateEnum.Created:
                State = new BalootSakkaCreatedState(this);
                break;
            case BalootSakkaStateEnum.Running:
                State = new BalootSakkaRunningState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                IsMashdoda = isMashdoda;
                DrawHandler = drawHandler;
                break;
            case BalootSakkaStateEnum.Paused:
                State = new BalootSakkaPausedState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                IsMashdoda = isMashdoda;
                DrawHandler = drawHandler;
                break;
            case BalootSakkaStateEnum.Ended:
                State = new BalootSakkaEndedState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                IsMashdoda = isMashdoda;
                DrawHandler = drawHandler;
                EndedAt = endedAt;
                Winner = winner;
                break;
            default:
                throw new System.ComponentModel.InvalidEnumArgumentException(nameof(stateName));
        }
    }
    private BalootSakkaState State { get; set; }
    public BalootSakkaStateEnum StateName => State.StateName;
    public BalootMoshtara CurrentMoshtara { get; set; }
    public List<BalootMoshtara> Moshtaras { get; set; }
    public bool IsMashdoda { get; set; }
    public BalootGameTeam? Winner { get; set; }
    public BalootDrawHandler DrawHandler { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public List<PausingInterval> PausingIntervals { get; set; }
    public int UsScore
    {
        get => Moshtaras.Aggregate(0, (totalScore, moshtara) => totalScore + moshtara.UsScore);
    }
    public int ThemScore
    {
        get => Moshtaras.Aggregate(0, (totalScore, moshtara) => totalScore + moshtara.ThemScore);
    }

    [JsonIgnore]
    public int WinningScore { get => IsMashdoda ? 100 : 152; }

    [JsonIgnore]
    public TimeSpan SakkaInterval
    {
        get
        {
            if (!IsEnded || EndedAt == null || StartedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt.Value - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }

    [JsonIgnore]
    public bool IsRunningWithMoshtaras => State is BalootSakkaRunningState && Moshtaras.Count > 0;

    [JsonIgnore]
    public bool IsCreated => State is BalootSakkaCreatedState;

    [JsonIgnore]
    public bool IsEnded => State is BalootSakkaEndedState;

    [JsonIgnore]
    public bool IsRunningWithoutMoshtaras => State is BalootSakkaRunningState && Moshtaras.Count == 0;

    #region methods
    public Result<BalootMoshtara> GetLastMoshtara()
    {
        var moshtara = Moshtaras.LastOrDefault();
        if (moshtara == null)
        {
            var err = new InvalidBalootGameActionError($"there is no moshtaras in the moshtaras list");
            return Result.Fail(err);
        }
        return Result.Ok(moshtara);
    }
    public BalootGameTeam? CheckSakkaWinner(BalootGameTeam selectedWinner, BalootDrawHandler handler = BalootDrawHandler.ExtraMoshtara)
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
    public BalootGameStatistics GetStatistics() =>
          Moshtaras
          .Aggregate(BalootGameStatistics.Zero(), (total, moshtara) => total + moshtara.GetStatistics())
          .AddSakkaResult(Winner);
    #endregion

    #region transactions 
    public Result StartMoshtara(DateTimeOffset startAt)
       => State.StartMoshtara(startAt);
    public Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        => State.EndMoshtara(moshtaraData, endAt);

    public Result Pause(DateTimeOffset pausedAt)
        => State.Pause(pausedAt);

    public Result Resume(DateTimeOffset resumedAt)
        => State.Resume(resumedAt);

    public Result Back(bool withRemoveLastMoshtara = true)
        => State.Back(withRemoveLastMoshtara);

    public Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        => State.UpdateMoshtara(moshtaraData, triggeredAt);

    public Result AddMashare3(int usScore, int themScore)
        => State.AddMashare3(usScore, themScore);

    public Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
        => State.StartSakka(isMashdoda, startAt);
    public Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
       => State.EndSakka(winner, drawHandler, triggeredAt);
    public Result ChangeIsSakkaMashdoda(bool isMashdoda)
        => State.ChangeIsSakkaMashdoda(isMashdoda);

    #endregion

    #region states
    public abstract class BalootSakkaState(BalootSakka sakka, BalootSakkaStateEnum stateName)
    {
        public BalootSakkaStateEnum StateName { get; } = stateName;
        public BalootSakka Sakka { get; } = sakka;
        protected InvalidBalootGameActionError InvalidTrigger(string triggerName)
           => new($"Can't Fire trigger : {triggerName} On Sakka Current State : {StateName}");

        #region state Transitions

        public virtual Result StartMoshtara(DateTimeOffset startAt)
            => Result.Fail(InvalidTrigger(nameof(StartMoshtara)));
        public virtual Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
            => Result.Fail(InvalidTrigger(nameof(EndMoshtara)));

        public virtual Result Pause(DateTimeOffset pausedAt)
            => Result.Fail(InvalidTrigger(nameof(Pause)));

        public virtual Result Resume(DateTimeOffset resumedAt)
            => Result.Fail(InvalidTrigger(nameof(Resume)));

        public virtual Result Back(bool withRemoveLastMoshtara = true)
            => Result.Fail(InvalidTrigger(nameof(Back)));

        public virtual Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
            => Result.Fail(InvalidTrigger(nameof(UpdateMoshtara)));

        public virtual Result AddMashare3(int usScore, int themScore)
            => Result.Fail(InvalidTrigger(nameof(AddMashare3)));

        public virtual Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
            => Result.Fail(InvalidTrigger(nameof(StartSakka)));
        public virtual Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
           => Result.Fail(InvalidTrigger(nameof(StartSakka)));

        public virtual Result ChangeIsSakkaMashdoda(bool isMashdoda)
        {
            Sakka.IsMashdoda = isMashdoda;
            return Result.Ok();
        }


        #endregion
    }
    public class BalootSakkaCreatedState(BalootSakka Sakka)
        : BalootSakkaState(Sakka, BalootSakkaStateEnum.Created)
    {
        public override Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
        {
            Sakka.StartedAt = startAt;
            Sakka.IsMashdoda = isMashdoda;
            Sakka.State = new BalootSakkaRunningState(Sakka);
            return Result.Ok();
        }



    }
    public class BalootSakkaRunningState(BalootSakka Sakka)
        : BalootSakkaState(Sakka, BalootSakkaStateEnum.Running)
    {

        public override Result Pause(DateTimeOffset pausedAt)
        {
            return Sakka.CurrentMoshtara.Pause(pausedAt)
            .OnSuccess(() =>
            {
                Sakka.PausingIntervals.Add(new(pausedAt, null));
                Sakka.State = new BalootSakkaPausedState(Sakka);
                return Result.Ok();
            });
        }
        public override Result StartMoshtara(DateTimeOffset startAt)
            => Sakka.CurrentMoshtara.StartMoshtara(startAt);
        public override Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        {
            return Sakka.CurrentMoshtara.EndMoshtara(moshtaraData, endAt)
            .OnSuccess(() =>
            {
                Sakka.Moshtaras.Add(Sakka.CurrentMoshtara);
                Sakka.CurrentMoshtara = new();
                return Result.Ok();
            });
        }
        public override Result Back(bool withRemoveLastMoshtara = true)
        {
            return Sakka.GetLastMoshtara()
                .OnSuccess((moshtara) =>
                {
                    Sakka.CurrentMoshtara = moshtara;
                    Sakka.Moshtaras.RemoveAt(Sakka.Moshtaras.Count - 1);
                    return Sakka.CurrentMoshtara.Back();
                });
        }
        public override Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
        {
            var calculatedWinner = Sakka.CheckSakkaWinner(winner, drawHandler);
            if (calculatedWinner is null)
                return Result.Fail(new InvalidBalootGameActionError(
                        $"Can't EndSakka with total UsScore : {Sakka.UsScore} total ThemScore : {Sakka.ThemScore} and draw handler : {drawHandler}"));
            Sakka.EndedAt = triggeredAt;
            Sakka.Winner = calculatedWinner;
            Sakka.DrawHandler = drawHandler;
            Sakka.State = new BalootSakkaEndedState(Sakka);
            return Result.Ok();
        }
        public override Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
            => Sakka.GetLastMoshtara()
                .OnSuccess((moshtara) => moshtara.UpdateMoshtara(moshtaraData, triggeredAt))
                .OnSuccess(() => Sakka.CurrentMoshtara = new());

        public override Result AddMashare3(int usScore, int themScore)
            => Sakka.GetLastMoshtara()
                .OnSuccess(moshtara => moshtara.AddMashare3(usScore, themScore));

    }
    public class BalootSakkaPausedState(BalootSakka Sakka)
        : BalootSakkaState(Sakka, BalootSakkaStateEnum.Paused)
    {

        public override Result Resume(DateTimeOffset resumedAt)
        {
            if (Sakka.PausingIntervals.Count == 0)
                throw new IndexOutOfRangeException("Sakka PausingIntervals doesn't have any values to get the last one.");
            Sakka.PausingIntervals[^1] = Sakka.PausingIntervals.Last() with { EndAt = resumedAt };
            Sakka.State = new BalootSakkaRunningState(Sakka);
            return Result.Ok();
        }
    }
    public class BalootSakkaEndedState(BalootSakka Sakka)
        : BalootSakkaState(Sakka, BalootSakkaStateEnum.Ended)
    {
        public override Result ChangeIsSakkaMashdoda(bool isMashdoda)
           => Result.Fail(InvalidTrigger(nameof(ChangeIsSakkaMashdoda)));

        public override Result Back(bool withRemoveLastMoshtara = true)
        {
            Sakka.Winner = null;
            Sakka.DrawHandler = BalootDrawHandler.ExtraMoshtara;
            Sakka.EndedAt = null;
            if (!withRemoveLastMoshtara)
            {
                Sakka.State = new BalootSakkaRunningState(Sakka);
                return Result.Ok();
            }
            return Sakka.GetLastMoshtara()
            .OnSuccess(moshtara =>
            {
                Sakka.CurrentMoshtara = moshtara;
                Sakka.Moshtaras.RemoveAt(Sakka.Moshtaras.Count - 1);
                return Sakka.CurrentMoshtara.Back();
            })
            .OnSuccess(() => Sakka.State = new BalootSakkaRunningState(Sakka));
        }
    }

    public enum BalootSakkaStateEnum
    {
        Created, Running, Paused, Ended
    }
    #endregion
}
