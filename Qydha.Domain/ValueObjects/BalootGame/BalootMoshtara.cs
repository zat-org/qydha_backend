namespace Qydha.Domain.ValueObjects;
public class BalootMoshtara
{
    public BalootMoshtara()
    {
        State = new BalootMoshtaraCreatedState(this);
        PausingIntervals = [];

    }

    [JsonConstructor]
    public BalootMoshtara(BalootMoshtaraStateEnum stateName, MoshtaraData? data, DateTimeOffset? startedAt, DateTimeOffset? endedAt, List<PausingInterval> pausingIntervals)
    {
        PausingIntervals = [];
        switch (stateName)
        {
            case BalootMoshtaraStateEnum.Created:
                State = new BalootMoshtaraCreatedState(this);
                break;
            case BalootMoshtaraStateEnum.Running:
                State = new BalootMoshtaraRunningState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                break;
            case BalootMoshtaraStateEnum.Paused:
                State = new BalootMoshtaraPausedState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                break;
            case BalootMoshtaraStateEnum.Ended:
                State = new BalootMoshtaraEndedState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                EndedAt = endedAt;
                Data = data;
                break;
            default:
                throw new System.ComponentModel.InvalidEnumArgumentException(nameof(stateName));
        }
    }

    public int UsScore { get => Data?.UsAbnat ?? 0; }
    public int ThemScore { get => Data?.ThemAbnat ?? 0; }
    public MoshtaraData? Data { get; set; }
    private BalootMoshtaraState State { get; set; }
    public BalootMoshtaraStateEnum StateName => State.StateName;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public List<PausingInterval> PausingIntervals { get; set; }

    [JsonIgnore]
    public TimeSpan MoshtaraInterval
    {
        get
        {
            if (!IsEnded || EndedAt == null || StartedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt.Value - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }

    [JsonIgnore]
    public bool IsCreated => State is BalootMoshtaraCreatedState;

    [JsonIgnore]
    public bool IsRunning => State is BalootMoshtaraRunningState;

    [JsonIgnore]
    public bool IsEnded => State is BalootMoshtaraEndedState;

    [JsonIgnore]
    public bool IsPaused => State is BalootMoshtaraPausedState;


    public BalootGameStatistics GetStatistics() => Data?.GetStatistics() ?? BalootGameStatistics.Zero();

    #region transitions 
    public Result StartMoshtara(DateTimeOffset startAt)
        => State.StartMoshtara(startAt);
    public Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        => State.EndMoshtara(moshtaraData, endAt);

    public Result Pause(DateTimeOffset pausedAt)
        => State.Pause(pausedAt);

    public Result Resume(DateTimeOffset resumedAt)
        => State.Resume(resumedAt);

    public Result Back()
        => State.Back();

    public Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        => State.UpdateMoshtara(moshtaraData, triggeredAt);

    public Result AddMashare3(int usScore, int themScore)
        => State.AddMashare3(usScore, themScore);
    #endregion

    public abstract class BalootMoshtaraState(BalootMoshtara moshtara, BalootMoshtaraStateEnum stateName)
    {
        public BalootMoshtaraStateEnum StateName { get; } = stateName;
        public BalootMoshtara Moshtara { get; } = moshtara;
        private InvalidBalootGameActionError InvalidTrigger(string triggerName)
           => new($"Can't Fire trigger : {triggerName} On Moshtara Current State : {StateName}");

        #region state behavior

        public virtual Result StartMoshtara(DateTimeOffset startAt)
            => Result.Fail(InvalidTrigger(nameof(StartMoshtara)));
        public virtual Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
            => Result.Fail(InvalidTrigger(nameof(EndMoshtara)));

        public virtual Result Pause(DateTimeOffset pausedAt)
            => Result.Fail(InvalidTrigger(nameof(Pause)));

        public virtual Result Resume(DateTimeOffset resumedAt)
            => Result.Fail(InvalidTrigger(nameof(Resume)));

        public virtual Result Back()
            => Result.Fail(InvalidTrigger(nameof(Back)));

        public virtual Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
            => Result.Fail(InvalidTrigger(nameof(UpdateMoshtara)));

        public virtual Result AddMashare3(int usScore, int themScore)
            => Result.Fail(InvalidTrigger(nameof(AddMashare3)));

        #endregion

    }
    public class BalootMoshtaraCreatedState(BalootMoshtara moshtara)
        : BalootMoshtaraState(moshtara, BalootMoshtaraStateEnum.Created)
    {
        public override Result StartMoshtara(DateTimeOffset startAt)
        {
            Moshtara.StartedAt = startAt;
            Moshtara.State = new BalootMoshtaraRunningState(Moshtara);
            return Result.Ok();
        }
    }
    public class BalootMoshtaraRunningState(BalootMoshtara moshtara)
        : BalootMoshtaraState(moshtara, BalootMoshtaraStateEnum.Running)
    {
        public override Result Pause(DateTimeOffset pausedAt)
        {
            Moshtara.PausingIntervals.Add(new(pausedAt, null));
            Moshtara.State = new BalootMoshtaraPausedState(Moshtara);
            return Result.Ok();
        }

        public override Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        {
            Moshtara.Data = moshtaraData;
            Moshtara.EndedAt = endAt;
            Moshtara.State = new BalootMoshtaraEndedState(Moshtara);
            return Result.Ok();
        }

    }
    public class BalootMoshtaraPausedState(BalootMoshtara moshtara)
        : BalootMoshtaraState(moshtara, BalootMoshtaraStateEnum.Paused)
    {
        public override Result Resume(DateTimeOffset resumedAt)
        {
            if (Moshtara.PausingIntervals.Count == 0)
                throw new IndexOutOfRangeException("Moshtara PausingIntervals doesn't have any values to get the last one.");
            Moshtara.PausingIntervals[^1] = Moshtara.PausingIntervals.Last() with { EndAt = resumedAt };
            Moshtara.State = new BalootMoshtaraRunningState(Moshtara);
            return Result.Ok();
        }
    }
    public class BalootMoshtaraEndedState(BalootMoshtara moshtara)
        : BalootMoshtaraState(moshtara, BalootMoshtaraStateEnum.Ended)
    {
        public override Result Back()
        {
            Moshtara.Data = null;
            Moshtara.EndedAt = null;
            Moshtara.State = new BalootMoshtaraRunningState(Moshtara);
            return Result.Ok();
        }
        public override Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        {
            Moshtara.Data = moshtaraData;
            Moshtara.EndedAt = triggeredAt;
            return Result.Ok();
        }
        public override Result AddMashare3(int usScore, int themScore)
        {
            if (Moshtara.Data == null)
                throw new NullReferenceException("Moshtara Data can't be null in state ended.");
            Moshtara.Data.AddMashare3(usScore, themScore);
            return Result.Ok();
        }
    }

    public enum BalootMoshtaraStateEnum
    {
        Created,
        Running,
        Paused,
        Ended
    }
}
