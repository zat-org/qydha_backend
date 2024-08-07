namespace Qydha.Domain.Entities;

public class BalootGameTimeLineBlock(TimeLineBlockType type, double triggeredAfterInSec, BalootGameTimeLineBlockTeamData usData, BalootGameTimeLineBlockTeamData themData)
{
    public TimeLineBlockType Type { get; set; } = type;
    public double TriggeredAfterInSec { get; set; } = triggeredAfterInSec;
    public BalootGameTimeLineBlockTeamData UsData { get; set; } = usData;
    public BalootGameTimeLineBlockTeamData ThemData { get; set; } = themData;
}

public class BalootGameTimeLineBlockTeamData(string name, int totalGameStore, int totalSakkaScore, int[] sakkaScores)
{
    public string Name { get; set; } = name;
    public int TotalGameScore { get; set; } = totalGameStore;
    public int TotalSakkaScore { get; set; } = totalSakkaScore;
    public int[] SakkaScores { get; set; } = sakkaScores;
}

public enum TimeLineBlockType
{
    ScoreWithWinner,
    ScoreWithoutWinner
}