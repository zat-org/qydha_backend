using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qydha.Domain.Entities;
public class BalootSakka
{
    private BalootSakka(BalootSakkaStateEnum stateName)
    {
        // State = new BalootSakkaCreatedState(this);
        StateName = stateName;
        _moshtaras = [];
        PausingIntervals = [];
    }
    public static BalootSakka CreateNewSakka()
    {
        return new BalootSakka(BalootSakkaStateEnum.Created)
        {
            Moshtaras = [new(BalootMoshtaraStateEnum.Created)]
        };
    }
    public int Id { get; set; }
    public Guid BalootGameId { get; set; }
    private BalootSakkaState State { get; set; } = null!;
    private BalootSakkaStateEnum _stateName;
    public BalootSakkaStateEnum StateName
    {
        get => _stateName;
        set
        {
            State = value switch
            {
                BalootSakkaStateEnum.Created => new BalootSakkaCreatedState(this),
                BalootSakkaStateEnum.Running => new BalootSakkaRunningState(this),
                BalootSakkaStateEnum.Paused => new BalootSakkaPausedState(this),
                BalootSakkaStateEnum.Ended => new BalootSakkaEndedState(this),
                _ => throw new InvalidEnumArgumentException(nameof(value)),
            };
            _stateName = value;
        }
    }
    [JsonIgnore]
    public BalootMoshtara CurrentMoshtara { get => Moshtaras.Last(); }

    private List<BalootMoshtara> _moshtaras;
    public List<BalootMoshtara> Moshtaras { get => _moshtaras; private set => _moshtaras = [.. value.OrderBy(m => m.Id)]; }
    public bool IsMashdoda { get; set; }
    public BalootGameTeam? Winner { get; set; }
    public BalootDrawHandler DrawHandler { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public List<PausingInterval> PausingIntervals { get; set; }

    public int UsScore { get; private set; }
    public int ThemScore { get; private set; }

    private void UpdateScores()
    {
        UsScore = Moshtaras.Aggregate(0, (totalScore, moshtara) => totalScore + moshtara.UsScore);
        ThemScore = Moshtaras.Aggregate(0, (totalScore, moshtara) => totalScore + moshtara.ThemScore);
    }

    [NotMapped]
    public int WinningScore { get => IsMashdoda ? 100 : 152; }

    [NotMapped]
    public TimeSpan SakkaInterval
    {
        get
        {
            if (!IsEnded || EndedAt == null || StartedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt.Value - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }

    [NotMapped]
    public bool IsRunningWithMoshtaras => State is BalootSakkaRunningState && Moshtaras.Count > 1;

    [NotMapped]
    public bool IsCreated => State is BalootSakkaCreatedState;

    [NotMapped]
    public bool IsEnded => State is BalootSakkaEndedState;

    [NotMapped]
    public bool IsRunningWithoutMoshtaras => State is BalootSakkaRunningState && Moshtaras.Count <= 1;

    #region methods
    public Result<BalootMoshtara> GetLastEndedMoshtara()
    {
        if (IsRunningWithoutMoshtaras)
        {
            return Result.Fail(
                new InvalidBalootGameActionError($"Can't Get last Ended Moshtara in sakka state : IsRunningWithoutMoshtaras"));
        }
        return Result.Ok(Moshtaras[^2]);
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
        => State.EndMoshtara(moshtaraData, endAt)
            .OnSuccess(UpdateScores);

    public Result Pause(DateTimeOffset pausedAt)
        => State.Pause(pausedAt);

    public Result Resume(DateTimeOffset resumedAt)
        => State.Resume(resumedAt);

    public Result Back(bool withRemoveLastMoshtara = true)
        => State.Back(withRemoveLastMoshtara)
            .OnSuccess(UpdateScores);

    public Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        => State.UpdateMoshtara(moshtaraData, triggeredAt)
            .OnSuccess(UpdateScores);

    public Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
        => State.AddMashare3(usScore, themScore, triggeredAt)
            .OnSuccess(UpdateScores);

    public Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
        => State.StartSakka(isMashdoda, startAt);
    public Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
       => State.EndSakka(winner, drawHandler, triggeredAt);
    public Result ChangeIsSakkaMashdoda(bool isMashdoda)
        => State.ChangeIsSakkaMashdoda(isMashdoda);

    public Result Reset() => State.Reset();
    #endregion

}

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

    public virtual Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
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
    public virtual Result Reset()
        => Result.Fail(InvalidTrigger(nameof(Reset)));

    #endregion
}
public class BalootSakkaCreatedState(BalootSakka Sakka)
    : BalootSakkaState(Sakka, BalootSakkaStateEnum.Created)
{
    public override Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
    {
        Sakka.StartedAt = startAt;
        Sakka.IsMashdoda = isMashdoda;
        Sakka.StateName = BalootSakkaStateEnum.Running;
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
            Sakka.StateName = BalootSakkaStateEnum.Paused;
            return Result.Ok();
        });
    }
    public override Result StartMoshtara(DateTimeOffset startAt)
    {
        if (Sakka.CheckSakkaWinner(selectedWinner: BalootGameTeam.Us) != null)
        {
            return Result.Fail(
            new InvalidBalootGameActionError(
                $"Can't Fire StartMoshtara When sakka has a winner, usScore : {Sakka.UsScore} , themScore : {Sakka.ThemScore}"
            ));
        }
        return Sakka.CurrentMoshtara.StartMoshtara(startAt);
    }
    public override Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
    {
        if (Sakka.CheckSakkaWinner(selectedWinner: BalootGameTeam.Us) != null)
        {
            return Result.Fail(
            new InvalidBalootGameActionError(
                $"Can't Fire EndMoshtara When sakka has a winner, usScore : {Sakka.UsScore} , themScore : {Sakka.ThemScore}"
            ));
        }
        return Sakka.CurrentMoshtara.EndMoshtara(moshtaraData, endAt)
            .OnSuccess(() => Sakka.Moshtaras.Add(new(BalootMoshtaraStateEnum.Created)));
    }
    public override Result Back(bool withRemoveLastMoshtara = true)
    {
        if (Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire Back in sakka state : IsRunningWithoutMoshtaras"));
        Sakka.Moshtaras.RemoveAt(Sakka.Moshtaras.Count - 1);
        return Sakka.CurrentMoshtara.Back();
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
        Sakka.StateName = BalootSakkaStateEnum.Ended;
        return Result.Ok();
    }
    public override Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        if (Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire UpdateMoshtara in sakka state : IsRunningWithoutMoshtaras"));

        return Sakka.Moshtaras[^2].UpdateMoshtara(moshtaraData, triggeredAt)
            .OnSuccess(() => Sakka.Moshtaras[^1] = new(BalootMoshtaraStateEnum.Created));
    }
    public override Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
    {
        if (Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire AddMashare3 in sakka state : IsRunningWithoutMoshtaras"));

        return Sakka.Moshtaras[^2].AddMashare3(usScore, themScore, triggeredAt)
            .OnSuccess(() => Sakka.Moshtaras[^1] = new(BalootMoshtaraStateEnum.Created));
    }

    public override Result Reset()
    {
        if (!Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire Reset in sakka state : IsRunningWithMoshtaras"));
        return Sakka.CurrentMoshtara.Reset()
            .OnSuccess(() =>
            {
                Sakka.StateName = BalootSakkaStateEnum.Created;
                Sakka.StartedAt = null;
                Sakka.PausingIntervals = [];
            });
    }
}

public class BalootSakkaPausedState(BalootSakka Sakka)
    : BalootSakkaState(Sakka, BalootSakkaStateEnum.Paused)
{

    public override Result Resume(DateTimeOffset resumedAt)
    {
        return Sakka.CurrentMoshtara.Resume(resumedAt)
        .OnSuccess(() =>
        {
            if (Sakka.PausingIntervals.Count == 0)
                throw new IndexOutOfRangeException("Sakka PausingIntervals doesn't have any values to get the last one.");
            Sakka.PausingIntervals[^1] = Sakka.PausingIntervals.Last() with { EndAt = resumedAt };
            Sakka.StateName = BalootSakkaStateEnum.Running;
        });
    }
}
public class BalootSakkaEndedState(BalootSakka Sakka)
    : BalootSakkaState(Sakka, BalootSakkaStateEnum.Ended)
{
    public override Result ChangeIsSakkaMashdoda(bool isMashdoda)
       => Result.Fail(InvalidTrigger(nameof(ChangeIsSakkaMashdoda)));

    public override Result Back(bool withRemoveLastMoshtara = true)
    {
        if (Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire Back in sakka state : IsRunningWithoutMoshtaras"));

        Sakka.Winner = null;
        Sakka.DrawHandler = BalootDrawHandler.ExtraMoshtara;
        Sakka.EndedAt = null;
        if (!withRemoveLastMoshtara)
        {
            Sakka.StateName = BalootSakkaStateEnum.Running;
            return Result.Ok();
        }
        Sakka.Moshtaras.RemoveAt(Sakka.Moshtaras.Count - 1);
        return Sakka.CurrentMoshtara.Back()
            .OnSuccess(() => Sakka.StateName = BalootSakkaStateEnum.Running);
    }
}

public enum BalootSakkaStateEnum
{
    Created, Running, Paused, Ended
}
#endregion