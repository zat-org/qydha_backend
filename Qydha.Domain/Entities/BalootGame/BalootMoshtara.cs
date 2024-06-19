using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qydha.Domain.Entities;
public class BalootMoshtara
{
    public BalootMoshtara(BalootMoshtaraStateEnum stateName)
    {
        StateName = stateName;
        PausingIntervals = [];
        // State = new BalootMoshtaraCreatedState(this);
    }
    public int Id { get; set; }
    public int BalootSakkaId { get; set; }
    private int _usScore;
    public int UsScore { get => _usScore; private set => _usScore = value; }

    private int _themScore;
    public int ThemScore { get => _themScore; private set => _themScore = value; }
    private MoshtaraData? _data;
    public MoshtaraData? Data
    {
        get => _data;
        set
        {
            _data = value;
            ThemScore = _data?.ThemAbnat ?? 0;
            UsScore = _data?.UsAbnat ?? 0;
        }
    }

    [NotMapped]
    private BalootMoshtaraState State { get; set; } = null!;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public List<PausingInterval> PausingIntervals { get; set; }

    private BalootMoshtaraStateEnum _stateName;
    public BalootMoshtaraStateEnum StateName
    {
        get => _stateName;
        set
        {
            State = value switch
            {
                BalootMoshtaraStateEnum.Created => new BalootMoshtaraCreatedState(this),
                BalootMoshtaraStateEnum.Running => new BalootMoshtaraRunningState(this),
                BalootMoshtaraStateEnum.Paused => new BalootMoshtaraPausedState(this),
                BalootMoshtaraStateEnum.Ended => new BalootMoshtaraEndedState(this),
                _ => throw new InvalidEnumArgumentException(nameof(value)),
            };
            _stateName = value;
        }
    }

    [NotMapped]
    public TimeSpan MoshtaraInterval
    {
        get
        {
            if (!IsEnded || EndedAt == null || StartedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt.Value - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }

    [NotMapped]
    public bool IsCreated => State is BalootMoshtaraCreatedState;

    [NotMapped]
    public bool IsRunning => State is BalootMoshtaraRunningState;

    [NotMapped]
    public bool IsEnded => State is BalootMoshtaraEndedState;

    [NotMapped]
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

    public Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
        => State.AddMashare3(usScore, themScore, triggeredAt);
    public Result Reset()
        => State.Reset();
    #endregion


}
#region states
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

    public virtual Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
        => Result.Fail(InvalidTrigger(nameof(AddMashare3)));
    public virtual Result Reset()
        => Result.Fail(InvalidTrigger(nameof(Reset)));

    #endregion

}
public class BalootMoshtaraCreatedState(BalootMoshtara moshtara)
    : BalootMoshtaraState(moshtara, BalootMoshtaraStateEnum.Created)
{
    public override Result StartMoshtara(DateTimeOffset startAt)
    {
        Moshtara.StartedAt = startAt;
        Moshtara.StateName = BalootMoshtaraStateEnum.Running;
        return Result.Ok();
    }
    public override Result Reset()
    {
        return Result.Ok();
    }
}
public class BalootMoshtaraRunningState(BalootMoshtara moshtara)
    : BalootMoshtaraState(moshtara, BalootMoshtaraStateEnum.Running)
{
    public override Result Pause(DateTimeOffset pausedAt)
    {
        Moshtara.PausingIntervals.Add(new(pausedAt, null));
        Moshtara.StateName = BalootMoshtaraStateEnum.Paused;
        return Result.Ok();
    }

    public override Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
    {
        Moshtara.Data = moshtaraData;
        Moshtara.EndedAt = endAt;
        Moshtara.StateName = BalootMoshtaraStateEnum.Ended;
        return Result.Ok();
    }
    public override Result Reset()
    {
        Moshtara.StateName = BalootMoshtaraStateEnum.Created;
        Moshtara.StartedAt = null;
        Moshtara.PausingIntervals = [];
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
        Moshtara.StateName = BalootMoshtaraStateEnum.Running;
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
        Moshtara.StateName = BalootMoshtaraStateEnum.Running;
        return Result.Ok();
    }
    public override Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        Moshtara.Data = moshtaraData;
        Moshtara.EndedAt = triggeredAt;
        return Result.Ok();
    }
    public override Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
    {
        if (Moshtara.Data == null)
            throw new NullReferenceException("Moshtara Data can't be null in state ended.");
        Moshtara.Data.AddMashare3(usScore, themScore);
        Moshtara.EndedAt = triggeredAt;
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
#endregion