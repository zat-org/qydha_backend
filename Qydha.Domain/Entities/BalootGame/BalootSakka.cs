using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qydha.Domain.Entities;
public class BalootSakka
{
    private BalootSakka(BalootSakkaStateEnum stateName)
    {
        StateName = stateName;
        _moshtaras = [];
        PausingIntervals = [];
    }
    public static BalootSakka CreateNewSakka(bool isMashdoda, DateTimeOffset startAt)
    {
        return new BalootSakka(BalootSakkaStateEnum.Running)
        {
            IsMashdoda = isMashdoda,
            StartedAt = startAt
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
    public DateTimeOffset StartedAt { get; set; }
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
            if (!IsEnded || EndedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }

    [NotMapped]
    public bool IsRunningWithMoshtaras => State is BalootSakkaRunningState && Moshtaras.Any(m => m.IsEnded);

    [NotMapped]
    public bool IsRunningWithoutMoshtaras => State is BalootSakkaRunningState && Moshtaras.All(m => !m.IsEnded);
  
    [NotMapped]
    public bool IsEnded => State is BalootSakkaEndedState;


    #region methods

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
    public Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
       => State.EndSakka(winner, drawHandler, triggeredAt);
    public Result ChangeIsSakkaMashdoda(bool isMashdoda)
        => State.ChangeIsSakkaMashdoda(isMashdoda);
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

    public virtual Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
       => Result.Fail(InvalidTrigger(nameof(EndSakka)));

    public virtual Result ChangeIsSakkaMashdoda(bool isMashdoda)
    {
        Sakka.IsMashdoda = isMashdoda;
        return Result.Ok();
    }

    #endregion
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
        Sakka.Moshtaras.Add(BalootMoshtara.CreateNewMoshtara(startAt));
        return Result.Ok();
    }
    public override Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
    {
        if (Sakka.Moshtaras.Count == 0)
            return Result.Fail(
                new InvalidBalootGameActionError(
                    $"Can't Fire EndMoshtara in sakka state : IsRunningWithoutMoshtaras"
                ));
        return Sakka.CurrentMoshtara.EndMoshtara(moshtaraData, endAt);
    }
    public override Result Back(bool withRemoveLastMoshtara = true)
    {
        if (Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire Back in sakka state : IsRunningWithoutMoshtaras"));
        if (Sakka.CurrentMoshtara.IsRunning)
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

        if (Sakka.CurrentMoshtara.IsRunning)
            Sakka.Moshtaras.RemoveAt(Sakka.Moshtaras.Count - 1);

        return Sakka.CurrentMoshtara.UpdateMoshtara(moshtaraData, triggeredAt);
    }
    public override Result AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
    {
        if (Sakka.IsRunningWithoutMoshtaras)
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire AddMashare3 in sakka state : IsRunningWithoutMoshtaras"));

        if (Sakka.CurrentMoshtara.IsRunning)
            Sakka.Moshtaras.RemoveAt(Sakka.Moshtaras.Count - 1);

        return Sakka.CurrentMoshtara.AddMashare3(usScore, themScore, triggeredAt);
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
        Sakka.Winner = null;
        Sakka.DrawHandler = BalootDrawHandler.ExtraMoshtara;
        Sakka.EndedAt = null;
        if (withRemoveLastMoshtara)
            return Sakka.CurrentMoshtara.Back()
                .OnSuccess(() => Sakka.StateName = BalootSakkaStateEnum.Running);
        else
        {
            Sakka.StateName = BalootSakkaStateEnum.Running;
            return Result.Ok();
        }

    }
}

public enum BalootSakkaStateEnum
{
    Running, Paused, Ended
}
#endregion