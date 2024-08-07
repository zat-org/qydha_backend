﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Qydha.Domain.Entities;

public class BalootGame
{
    private BalootGame(BalootGameStateEnum stateName)
    {
        EventsJsonString = "[]";
        StateName = stateName;
        _sakkas = [];
        UsName = "Us";
        ThemName = "Them";
        PausingIntervals = [];
    }
    public static BalootGame CreateSinglePlayerGame(Guid ownerId, DateTimeOffset createdAt, XInfoData deviceData)
    {
        return new BalootGame(BalootGameStateEnum.Created)
        {
            GameMode = BalootGameMode.SinglePlayer,
            CreatedAt = createdAt.ToUniversalTime(),
            OwnerId = ownerId,
            ModeratorId = ownerId,
            DeviceData = deviceData
        };
    }
    public static BalootGame CreateAnonymousGame(DateTimeOffset createdAt, XInfoData deviceData)
    {
        return new BalootGame(BalootGameStateEnum.Created)
        {
            GameMode = BalootGameMode.AnonymousGame,
            CreatedAt = createdAt.ToUniversalTime(),
            OwnerId = null,
            ModeratorId = null,
            DeviceData = deviceData
        };
    }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string EventsJsonString { get; set; }
    public BalootGameMode GameMode { get; set; }
    public Guid? ModeratorId { get; set; }
    public virtual User? Moderator { get; set; }
    public Guid? OwnerId { get; set; }
    public virtual User? Owner { get; set; }
    private BalootGameState State { get; set; } = null!;

    public XInfoData DeviceData { get; init; } = null!;

    private BalootGameStateEnum _stateName;
    public BalootGameStateEnum StateName
    {
        get => _stateName;
        set
        {
            State = value switch
            {
                BalootGameStateEnum.Created => new BalootGameCreatedState(this),
                BalootGameStateEnum.Running => new BalootGameRunningState(this),
                BalootGameStateEnum.Paused => new BalootGamePausedState(this),
                BalootGameStateEnum.Ended => new BalootGameEndedState(this),
                _ => throw new InvalidEnumArgumentException(nameof(value)),
            };
            _stateName = value;
        }
    }
    public string UsName { get; set; }
    public string ThemName { get; set; }
    public int MaxSakkaPerGame { get; set; }
    [JsonIgnore]
    public BalootSakka CurrentSakka { get => Sakkas.Last(); }
    private List<BalootSakka> _sakkas;
    public virtual List<BalootSakka> Sakkas { get => _sakkas; private set => _sakkas = [.. value.OrderBy(s => s.Id)]; }
    public BalootGameTeam? Winner { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public List<PausingInterval> PausingIntervals { get; set; }
    public int UsGameScore { get; private set; }
    public int ThemGameScore { get; private set; }
    public Point? Location { get; set; }

    private void UpdateGameScores()
    {
        UsGameScore = Sakkas.Aggregate(0, (totalScore, sakka) => totalScore + (sakka.Winner != null && sakka.Winner == BalootGameTeam.Us ? 1 : 0));
        ThemGameScore = Sakkas.Aggregate(0, (totalScore, sakka) => totalScore + (sakka.Winner != null && sakka.Winner == BalootGameTeam.Them ? 1 : 0));
    }

    [NotMapped]
    public TimeSpan GameInterval
    {
        get
        {
            if (!IsEnded || EndedAt == null || StartedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt.Value - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }
    [NotMapped]
    public bool IsRunningWithSakkas => State is BalootGameRunningState && Sakkas.Any(s => s.IsEnded);

    [NotMapped]

    public bool IsCreated => State is BalootGameCreatedState;

    [NotMapped]
    public bool IsEnded => State is BalootGameEndedState;

    [NotMapped]
    public bool IsRunningWithoutSakkas => State is BalootGameRunningState && Sakkas.All(s => !s.IsEnded);

    #region Methods
    public List<BalootGameEvent> GetEvents() =>
           JsonConvert.DeserializeObject<List<BalootGameEvent>>(EventsJsonString, BalootConstants.balootEventsSerializationSettings) ?? [];

    public Result CanSakkasCountPerGameChangeTo(int newSakkasCount)
    {
        if (newSakkasCount >= MaxSakkaPerGame || (newSakkasCount >= Sakkas.Count && CheckGameWinner(newSakkasCount) == null))
            return Result.Ok();
        return Result.Fail(new InvalidBalootGameActionError("Invalid MaxSakkaCountPerGame => doesn't lead to a non winner state."));
    }

    public BalootGameTeam? CheckGameWinner(int? newMaxSakkaPerGame = null)
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

    public BalootGameStatistics GetStatistics() =>
        CurrentSakka.GetStatistics() +
            Sakkas.Aggregate(BalootGameStatistics.Zero(), (total, sakka) => total + sakka.GetStatistics());

    #endregion

    #region state Transitions
    public Result<BalootGameEventEffect> StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt, Point? location)
        => State.StartGame(usName, themName, sakkaCount, triggeredAt, location);
    public Result<BalootGameEventEffect> EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
        => State.EndGame(winner, triggeredAt);
    public Result<BalootGameEventEffect> StartMoshtara(DateTimeOffset startAt)
        => State.StartMoshtara(startAt);
    public Result<BalootGameEventEffect> EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        => State.EndMoshtara(moshtaraData, endAt);
    public Result<BalootGameEventEffect> StartSakka(bool isMashdoda, DateTimeOffset startAt)
         => State.StartSakka(isMashdoda, startAt);
    public Result<BalootGameEventEffect> EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
        => State.EndSakka(winner, drawHandler, triggeredAt)
            .OnSuccess((effect) =>
            {
                UpdateGameScores();
                return Result.Ok(effect);
            });
    public Result<BalootGameEventEffect> Pause(DateTimeOffset pausedAt)
        => State.Pause(pausedAt);
    public Result<BalootGameEventEffect> Back()
        => State.Back()
            .OnSuccess((effect) =>
            {
                UpdateGameScores();
                return Result.Ok(effect);
            });
    public Result<BalootGameEventEffect> UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        => State.UpdateMoshtara(moshtaraData, triggeredAt);
    public Result<BalootGameEventEffect> AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
        => State.AddMashare3(usScore, themScore, triggeredAt);
    public Result<BalootGameEventEffect> Resume(DateTimeOffset resumedAt)
        => State.Resume(resumedAt);
    public Result<BalootGameEventEffect> ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
        => State.ChangeIsCurrentSakkaMashdoda(isMashdoda);
    public Result<BalootGameEventEffect> ChangeTeamsNames(string usName, string themName)
        => State.ChangeTeamsNames(usName, themName);
    public Result<BalootGameEventEffect> ChangeSakkaCount(int newSakkaCount)
        => State.ChangeSakkaCount(newSakkaCount);

    #endregion

}


public class InvalidBalootGameActionError(string msg)
    : ResultError(msg, ErrorType.InvalidBalootGameAction, StatusCodes.Status400BadRequest)
{ }

public record PausingInterval(DateTimeOffset StartAt, DateTimeOffset? EndAt);


#region  states
public abstract class BalootGameState(BalootGame game, BalootGameStateEnum stateName)
{
    public BalootGameStateEnum StateName { get; } = stateName;
    public BalootGame Game { get; } = game;
    protected InvalidBalootGameActionError InvalidTrigger(string triggerName)
       => new($"Can't Fire trigger : {triggerName} On Game Current State : {StateName}");

    #region state Transitions
    public virtual Result<BalootGameEventEffect> StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt, Point? Location)
        => Result.Fail(InvalidTrigger(nameof(StartGame)));
    public virtual Result<BalootGameEventEffect> EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
        => Result.Fail(InvalidTrigger(nameof(EndGame)));
    public virtual Result<BalootGameEventEffect> StartMoshtara(DateTimeOffset startAt)
        => Result.Fail(InvalidTrigger(nameof(StartMoshtara)));
    public virtual Result<BalootGameEventEffect> EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        => Result.Fail(InvalidTrigger(nameof(EndMoshtara)));
    public virtual Result<BalootGameEventEffect> StartSakka(bool isMashdoda, DateTimeOffset startAt)
        => Result.Fail(InvalidTrigger(nameof(StartSakka)));
    public virtual Result<BalootGameEventEffect> EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
        => Result.Fail(InvalidTrigger(nameof(StartSakka)));
    public virtual Result<BalootGameEventEffect> Pause(DateTimeOffset pausedAt)
        => Result.Fail(InvalidTrigger(nameof(Pause)));

    public virtual Result<BalootGameEventEffect> Back()
        => Result.Fail(InvalidTrigger(nameof(Back)));

    public virtual Result<BalootGameEventEffect> UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        => Result.Fail(InvalidTrigger(nameof(UpdateMoshtara)));

    public virtual Result<BalootGameEventEffect> AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt)
        => Result.Fail(InvalidTrigger(nameof(AddMashare3)));
    public virtual Result<BalootGameEventEffect> Resume(DateTimeOffset resumedAt)
        => Result.Fail(InvalidTrigger(nameof(Resume)));
    public virtual Result<BalootGameEventEffect> ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
        => Result.Fail(InvalidTrigger(nameof(ChangeIsCurrentSakkaMashdoda)));

    public virtual Result<BalootGameEventEffect> ChangeTeamsNames(string usName, string themName)
    {
        Game.UsName = usName;
        Game.ThemName = themName;
        return Result.Ok();
    }
    public virtual Result<BalootGameEventEffect> ChangeSakkaCount(int newSakkaCount)
    {
        return Game.CanSakkasCountPerGameChangeTo(newSakkaCount)
        .OnSuccess(() =>
        {
            Game.MaxSakkaPerGame = newSakkaCount;
            return Result.Ok(BalootGameEventEffect.MaxSakkaCountChanged);
        });
    }

    #endregion
}
public class BalootGameCreatedState(BalootGame game)
    : BalootGameState(game, BalootGameStateEnum.Created)
{
    public override Result<BalootGameEventEffect> StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt, Point? location)
    {
        Game.UsName = usName;
        Game.ThemName = themName;
        Game.MaxSakkaPerGame = sakkaCount;
        Game.StartedAt = triggeredAt;
        Game.StateName = BalootGameStateEnum.Running;
        Game.Location = location;
        return Result.Ok(BalootGameEventEffect.NamesChanged | BalootGameEventEffect.MaxSakkaCountChanged);
    }
}
public class BalootGameRunningState(BalootGame game)
    : BalootGameState(game, BalootGameStateEnum.Running)
{
    public override Result<BalootGameEventEffect> ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
        => Game.CurrentSakka.ChangeIsSakkaMashdoda(isMashdoda);
    public override Result<BalootGameEventEffect> StartMoshtara(DateTimeOffset startAt)
        => Game.CurrentSakka.StartMoshtara(startAt);
    public override Result<BalootGameEventEffect> EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        => Game.CurrentSakka.EndMoshtara(moshtaraData, endAt);
    public override Result<BalootGameEventEffect> StartSakka(bool isMashdoda, DateTimeOffset startAt)
    {
        if (Game.CheckGameWinner() != null)
        {
            return Result.Fail(new InvalidBalootGameActionError(
                $"Can't StartSakka when game has a winner,total UsScore : {Game.UsGameScore} total ThemScore : {Game.ThemGameScore}"
            ));
        }
        Game.Sakkas.Add(BalootSakka.CreateNewSakka(isMashdoda, startAt));
        return Result.Ok(BalootGameEventEffect.IsCurrentSakkaMashdodaChanged);
    }
    public override Result<BalootGameEventEffect> EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
        => Game.CurrentSakka.EndSakka(winner, drawHandler, triggeredAt);
    public override Result<BalootGameEventEffect> EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
    {
        var calculatedWinner = Game.CheckGameWinner();
        if (calculatedWinner is null)
            return Result.Fail(new InvalidBalootGameActionError(
                $"Can't EndGame with total UsScore : {Game.UsGameScore} total ThemScore : {Game.ThemGameScore}"));
        Game.Winner = calculatedWinner;
        Game.EndedAt = triggeredAt;
        Game.StateName = BalootGameStateEnum.Ended;
        return Result.Ok(BalootGameEventEffect.GameEnded);
    }

    public override Result<BalootGameEventEffect> Pause(DateTimeOffset pausedAt)
    {
        var res = Result.Ok(BalootGameEventEffect.NoChange);
        if (!Game.CurrentSakka.IsEnded)
            res = Game.CurrentSakka.Pause(pausedAt);
        return res.OnSuccess((effect) =>
        {
            Game.PausingIntervals.Add(new(pausedAt, null));
            Game.StateName = BalootGameStateEnum.Paused;
            return Result.Ok(effect);
        });
    }
    public override Result<BalootGameEventEffect> Back()
    {
        if (Game.IsRunningWithSakkas && Game.CurrentSakka.IsRunningWithoutMoshtaras)
        {
            Game.Sakkas.RemoveAt(Game.Sakkas.Count - 1);
            return Result.Ok(BalootGameEventEffect.ScoreChanges);
        }
        return Game.CurrentSakka.Back();
    }

    public override Result<BalootGameEventEffect> UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        if (Game.IsRunningWithSakkas && Game.CurrentSakka.IsEnded)
            return Game.CurrentSakka.Back(withRemoveLastMoshtara: false)
                .OnSuccess((effect) => Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt)
                    .ToResult(updateEffect => updateEffect | effect));
        return Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt);
    }

    public override Result<BalootGameEventEffect> AddMashare3(int usScore, int themScore, DateTimeOffset triggeredAt) =>
        Game.CurrentSakka.AddMashare3(usScore, themScore, triggeredAt);

}
public class BalootGamePausedState(BalootGame game)
    : BalootGameState(game, BalootGameStateEnum.Paused)
{
    public override Result<BalootGameEventEffect> Resume(DateTimeOffset resumedAt)
    {
        var res = Result.Ok(BalootGameEventEffect.NoChange);
        if (!Game.CurrentSakka.IsEnded)
            res = Game.CurrentSakka.Resume(resumedAt);
        return res.OnSuccess((effect) =>
        {
            if (Game.PausingIntervals.Count == 0)
                throw new IndexOutOfRangeException("Game PausingIntervals doesn't have any values to get the last one.");
            Game.PausingIntervals[^1] = Game.PausingIntervals.Last() with { EndAt = resumedAt };
            Game.StateName = BalootGameStateEnum.Running;
            return Result.Ok(effect);
        });
    }
    public override Result<BalootGameEventEffect> ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
        => Game.CurrentSakka.ChangeIsSakkaMashdoda(isMashdoda);
}
public class BalootGameEndedState(BalootGame game)
    : BalootGameState(game, BalootGameStateEnum.Ended)
{
    public override Result<BalootGameEventEffect> Back()
    {
        Game.Winner = null;
        Game.EndedAt = null;
        return Game.CurrentSakka.Back()
           .OnSuccess((effect) =>
            {
                Game.StateName = BalootGameStateEnum.Running;
                return Result.Ok(effect);
            });
    }
    public override Result<BalootGameEventEffect> UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        Game.Winner = null;
        Game.EndedAt = null;
        return Game.CurrentSakka.Back(withRemoveLastMoshtara: false)
                .OnSuccess((effect) => Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt)
                    .ToResult((updateEffect) => effect | updateEffect))
                .OnSuccess((effect) =>
                {
                    Game.StateName = BalootGameStateEnum.Running;
                    return Result.Ok(effect);
                });
    }
    public override Result<BalootGameEventEffect> ChangeSakkaCount(int newSakkaCount)
    {
        return base.ChangeSakkaCount(newSakkaCount)
            .OnSuccess((effect) =>
            {
                Game.Winner = null;
                Game.EndedAt = null;
                Game.StateName = BalootGameStateEnum.Running;
                return Result.Ok(effect);
            });
    }
}

public enum BalootGameStateEnum
{
    Created, Running, Paused, Ended
}
#endregion
