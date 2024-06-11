namespace Qydha.Domain.ValueObjects;
public record PausingInterval(DateTimeOffset StartAt, DateTimeOffset? EndAt);
public class BalootGameData
{
    public BalootGameData()
    {
        CurrentSakka = new();
        Sakkas = [];
        UsName = "لنا";
        ThemName = "لهم";
        MaxSakkaPerGame = 1;
        PausingIntervals = [];
        State = new BalootGameCreatedState(this);
    }

    [JsonConstructor]
    public BalootGameData(BalootGameStateEnum stateName, BalootSakka currentSakka, List<BalootSakka> sakkas, string usName, string themName, BalootGameTeam? winner, int maxSakkaPerGame, DateTimeOffset? startedAt, DateTimeOffset? endedAt, List<PausingInterval> pausingIntervals)
    {
        CurrentSakka = currentSakka;
        Sakkas = sakkas;
        UsName = "لنا";
        ThemName = "لهم";
        PausingIntervals = [];
        switch (stateName)
        {
            case BalootGameStateEnum.Created:
                State = new BalootGameCreatedState(this);
                break;
            case BalootGameStateEnum.Running:
                State = new BalootGameRunningState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                UsName = usName;
                ThemName = themName;
                MaxSakkaPerGame = maxSakkaPerGame;
                break;
            case BalootGameStateEnum.Paused:
                State = new BalootGamePausedState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                UsName = usName;
                ThemName = themName;
                MaxSakkaPerGame = maxSakkaPerGame;
                break;
            case BalootGameStateEnum.Ended:
                State = new BalootGameEndedState(this);
                StartedAt = startedAt;
                PausingIntervals = pausingIntervals;
                UsName = usName;
                ThemName = themName;
                MaxSakkaPerGame = maxSakkaPerGame;
                EndedAt = endedAt;
                Winner = winner;
                break;
            default:
                throw new System.ComponentModel.InvalidEnumArgumentException(nameof(stateName));
        }
    }
    private BalootGameState State { get; set; }
    public BalootGameStateEnum StateName { get => State.StateName; }
    public string UsName { get; set; }
    public string ThemName { get; set; }
    public int MaxSakkaPerGame { get; set; }
    public BalootSakka CurrentSakka { get; set; }
    public List<BalootSakka> Sakkas { get; set; }
    public BalootGameTeam? Winner { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public List<PausingInterval> PausingIntervals { get; set; }
    public int UsGameScore
    {
        get => Sakkas.Aggregate(0, (totalScore, sakka) => totalScore + (sakka.Winner != null && sakka.Winner == BalootGameTeam.Us ? 1 : 0));
    }
    public int ThemGameScore
    {
        get => Sakkas.Aggregate(0, (totalScore, sakka) => totalScore + (sakka.Winner != null && sakka.Winner == BalootGameTeam.Them ? 1 : 0));
    }

    [JsonIgnore]
    public TimeSpan GameInterval
    {
        get
        {
            if (!IsEnded || EndedAt == null || StartedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt.Value - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.StartAt - interval.EndAt!.Value));
        }
    }
    [JsonIgnore]
    public bool IsRunningWithSakkas => State is BalootGameRunningState && Sakkas.Count > 0;

    [JsonIgnore]
    public bool IsCreated => State is BalootGameCreatedState;

    [JsonIgnore]
    public bool IsEnded => State is BalootGameEndedState;

    [JsonIgnore]
    public bool IsRunningWithoutSakkas => State is BalootGameRunningState && Sakkas.Count == 0;

    #region Methods
    public Result CanSakkasCountPerGameChangeTo(int newSakkasCount)
    {
        if (newSakkasCount >= MaxSakkaPerGame || (newSakkasCount > Sakkas.Count && CheckGameWinner(newSakkasCount) == null))
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
    public Result<BalootSakka> GetLastSakka()
    {
        var sakka = Sakkas.LastOrDefault();
        if (sakka == null)
        {
            var err = new InvalidBalootGameActionError($"there is no sakkas in the sakkas list");
            return Result.Fail(err);
        }
        return Result.Ok(sakka);
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
    public Result StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt)
        => State.StartGame(usName, themName, sakkaCount, triggeredAt);
    public Result EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
        => State.EndGame(winner, triggeredAt);
    public Result StartMoshtara(DateTimeOffset startAt)
        => State.StartMoshtara(startAt);
    public Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
        => State.EndMoshtara(moshtaraData, endAt);
    public Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
         => State.StartSakka(isMashdoda, startAt);
    public Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
        => State.EndSakka(winner, drawHandler, triggeredAt);
    public Result Pause(DateTimeOffset pausedAt)
        => State.Pause(pausedAt);

    public Result Back() => State.Back();
    public Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        => State.UpdateMoshtara(moshtaraData, triggeredAt);

    public Result AddMashare3(int usScore, int themScore)
        => State.AddMashare3(usScore, themScore);
    public Result Resume(DateTimeOffset resumedAt)
        => State.Resume(resumedAt);
    public Result ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
        => State.ChangeIsCurrentSakkaMashdoda(isMashdoda);

    public Result ChangeTeamsNames(string usName, string themName)
        => State.ChangeTeamsNames(usName, themName);
    public Result ChangeSakkaCount(int newSakkaCount)
        => State.ChangeSakkaCount(newSakkaCount);

    #endregion

    #region  states
    public abstract class BalootGameState(BalootGameData game, BalootGameStateEnum stateName)
    {
        public BalootGameStateEnum StateName { get; } = stateName;
        public BalootGameData Game { get; } = game;
        protected InvalidBalootGameActionError InvalidTrigger(string triggerName)
           => new($"Can't Fire trigger : {triggerName} On Game Current State : {StateName}");

        #region state Transitions
        public virtual Result StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt)
            => Result.Fail(InvalidTrigger(nameof(StartGame)));
        public virtual Result EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
            => Result.Fail(InvalidTrigger(nameof(EndGame)));
        public virtual Result StartMoshtara(DateTimeOffset startAt)
            => Result.Fail(InvalidTrigger(nameof(StartMoshtara)));
        public virtual Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
            => Result.Fail(InvalidTrigger(nameof(EndMoshtara)));
        public virtual Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
            => Result.Fail(InvalidTrigger(nameof(StartSakka)));
        public virtual Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
            => Result.Fail(InvalidTrigger(nameof(StartSakka)));
        public virtual Result Pause(DateTimeOffset pausedAt)
            => Result.Fail(InvalidTrigger(nameof(Pause)));

        public virtual Result Back()
            => Result.Fail(InvalidTrigger(nameof(Back)));

        public virtual Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
            => Result.Fail(InvalidTrigger(nameof(UpdateMoshtara)));

        public virtual Result AddMashare3(int usScore, int themScore)
            => Result.Fail(InvalidTrigger(nameof(AddMashare3)));
        public virtual Result Resume(DateTimeOffset resumedAt)
            => Result.Fail(InvalidTrigger(nameof(Resume)));
        public virtual Result ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
            => Result.Fail(InvalidTrigger(nameof(ChangeIsCurrentSakkaMashdoda)));

        public virtual Result ChangeTeamsNames(string usName, string themName)
        {
            Game.UsName = usName;
            Game.ThemName = themName;
            return Result.Ok();
        }
        public virtual Result ChangeSakkaCount(int newSakkaCount)
        {
            return Game.CanSakkasCountPerGameChangeTo(newSakkaCount)
            .OnSuccess(() =>
            {
                Game.MaxSakkaPerGame = newSakkaCount;
            });
        }

        #endregion
    }
    public class BalootGameCreatedState(BalootGameData game)
        : BalootGameState(game, BalootGameStateEnum.Created)
    {
        public override Result StartGame(string usName, string themName, int sakkaCount, DateTimeOffset triggeredAt)
        {
            Game.UsName = usName;
            Game.ThemName = themName;
            Game.MaxSakkaPerGame = sakkaCount;
            Game.StartedAt = triggeredAt;
            Game.State = new BalootGameRunningState(Game);
            return Result.Ok();
        }
    }
    public class BalootGameRunningState(BalootGameData game)
        : BalootGameState(game, BalootGameStateEnum.Running)
    {
        public override Result EndGame(BalootGameTeam winner, DateTimeOffset triggeredAt)
        {
            var calculatedWinner = Game.CheckGameWinner();
            if (calculatedWinner is null)
                return Result.Fail(new InvalidBalootGameActionError(
                    $"Can't EndGame with total UsScore : {Game.UsGameScore} total ThemScore : {Game.ThemGameScore}"));
            Game.Winner = calculatedWinner;
            Game.EndedAt = triggeredAt;
            Game.State = new BalootGameEndedState(Game);
            return Result.Ok();
        }
        public override Result ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
            => Game.CurrentSakka.ChangeIsSakkaMashdoda(isMashdoda);
        public override Result StartMoshtara(DateTimeOffset startAt)
            => Game.CurrentSakka.StartMoshtara(startAt);
        public override Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset endAt)
            => Game.CurrentSakka.EndMoshtara(moshtaraData, endAt);
        public override Result StartSakka(bool isMashdoda, DateTimeOffset startAt)
            => Game.CurrentSakka.StartSakka(isMashdoda, startAt);
        public override Result EndSakka(BalootGameTeam winner, BalootDrawHandler drawHandler, DateTimeOffset triggeredAt)
        {
            return Game.CurrentSakka.EndSakka(winner, drawHandler, triggeredAt)
            .OnSuccess(() =>
            {
                Game.Sakkas.Add(Game.CurrentSakka);
                Game.CurrentSakka = new();
            });
        }
        public override Result Pause(DateTimeOffset pausedAt)
        {
            return Game.CurrentSakka.Pause(pausedAt)
            .OnSuccess(() =>
            {
                Game.PausingIntervals.Add(new(pausedAt, null));
                Game.State = new BalootGamePausedState(Game);
            });
        }

        public override Result Back()
        {
            if (Game.IsRunningWithoutSakkas || (Game.IsRunningWithSakkas && Game.CurrentSakka.IsRunningWithMoshtaras))
                return Game.CurrentSakka.Back();

            // game here is running with sakkas and the current sakka has not moshtaras (created or running without moshtaras) 
            return Game.GetLastSakka()
                .OnSuccess((sakka) =>
                {
                    Game.CurrentSakka = sakka;
                    Game.Sakkas.RemoveAt(Game.Sakkas.Count - 1);
                    return Game.CurrentSakka.Back(withRemoveLastMoshtara: !Game.CurrentSakka.IsRunningWithoutMoshtaras);
                });
        }

        public override Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        {
            if (Game.IsRunningWithoutSakkas)
                return Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt);

            // game here running with sakkas  
            if (!Game.CurrentSakka.IsCreated)
                return Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt);

            // game here running with sakkas and current sakka is created 

            return Game.GetLastSakka()
            .OnSuccess((sakka) =>
            {
                Game.CurrentSakka = sakka;
                Game.Sakkas.RemoveAt(Game.Sakkas.Count - 1);
                return Game.CurrentSakka
                        .Back(withRemoveLastMoshtara: false)
                        .OnSuccess(() => Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt));
            });
        }

        public override Result AddMashare3(int usScore, int themScore)
            => Game.CurrentSakka.AddMashare3(usScore, themScore);
    }
    public class BalootGamePausedState(BalootGameData game)
        : BalootGameState(game, BalootGameStateEnum.Paused)
    {
        public override Result Resume(DateTimeOffset resumedAt)
        {
            return Game.CurrentSakka.Resume(resumedAt)
            .OnSuccess(() =>
            {
                if (Game.PausingIntervals.Count == 0)
                    throw new IndexOutOfRangeException("Game PausingIntervals doesn't have any values to get the last one.");
                Game.PausingIntervals[^1] = Game.PausingIntervals.Last() with { EndAt = resumedAt };
                Game.State = new BalootGameRunningState(Game);
            });
        }
        public override Result ChangeIsCurrentSakkaMashdoda(bool isMashdoda)
            => Game.CurrentSakka.ChangeIsSakkaMashdoda(isMashdoda);
    }
    public class BalootGameEndedState(BalootGameData game)
        : BalootGameState(game, BalootGameStateEnum.Ended)
    {
        public override Result Back()
        {
            return Game.GetLastSakka()
            .OnSuccess((sakka) =>
            {
                Game.Winner = null;
                Game.EndedAt = null;
                Game.CurrentSakka = sakka;
                Game.Sakkas.RemoveAt(Game.Sakkas.Count - 1);
                return Game.CurrentSakka.Back();
            });
        }
        public override Result UpdateMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
        {
            return Game.GetLastSakka()
            .OnSuccess(sakka =>
            {
                Game.Winner = null;
                Game.EndedAt = null;
                Game.CurrentSakka = sakka;
                Game.Sakkas.RemoveAt(Game.Sakkas.Count - 1);
                return Game.CurrentSakka
                        .Back(withRemoveLastMoshtara: false)
                        .OnSuccess(() => Game.CurrentSakka.UpdateMoshtara(moshtaraData, triggeredAt))
                        .OnSuccess(() => Game.State = new BalootGameRunningState(Game));
            });
        }
    }

    public enum BalootGameStateEnum
    {
        Created, Running, Paused, Ended
    }
    #endregion

}

