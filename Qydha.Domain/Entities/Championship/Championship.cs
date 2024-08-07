using NetTopologySuite.Geometries;

namespace Qydha.Domain.Entities;
public class Championship
{
    //TODO Add lows
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int[] Prizes { get; set; } = null!;
    public string PrizesCurrencyName { get; set; } = null!;
    public FinalStage FinalStage { get; set; } = null!;
    public GroupStage? GroupStage { get; set; }
}
public class ChampionshipTeamJoinRequest
{

    public int Id { get; set; }
    public string FirstPlayerName { get; set; } = null!;
    public string FirstPlayerPhone { get; set; } = null!;
    public string SecondPlayerName { get; set; } = null!;
    public string SecondPlayerPhone { get; set; } = null!;
    public int ChampionshipId { get; set; }
    public Championship Championship { get; set; } = null!;
}
public abstract class Stage(StageType type)
{
    public StageType Type { get; set; } = type;
    public string City { get; set; } = null!;
    public Point Location { get; set; } = null!;
    public DateOnly StartAt { get; set; }
    public DateOnly EndAt { get; set; }
    public List<ChampionshipTeam> QualifiedTeams { get; set; } = [];
}
public class FinalStage() : Stage(StageType.Final)
{
    public bool With3rdPlaceMatch { get; set; }
    public int NumberOfParticipants { get; set; }
    public List<ChampionshipMatch> Matches { get; set; } = [];
    public List<ChampionshipTeam> Winners { get; set; } = [];
}

public class GroupStage() : Stage(StageType.Group)
{
    public int QualifiedTeamsCountPerGroup { get; set; }
    public List<ChampionshipQualificationGroup> Groups { get; set; } = [];
}
public class ChampionshipTeam
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid FirstPlayerId { get; set; }
    // public User FirstPlayer { get; set; } = null!;
    public Guid SecondPlayerId { get; set; }
    // public User SecondPlayer { get; set; } = null!;
    public int ChampionshipId { get; set; }
    public Championship Championship { get; set; } = null!;
}
public class ChampionshipMatch
{
    public DateTimeOffset StartAt { get; set; }
    public ChampionshipTeam UsTeam { get; set; } = null!;
    public ChampionshipTeam ThemTeam { get; set; } = null!;
    public BalootGame Game { get; set; } = null!;

}
public class ChampionshipQualificationGroup
{
    public int Id { get; set; }

    public List<ChampionshipMatch> Matches { get; set; } = [];
    public List<ChampionshipTeam> Winners { get; set; } = [];

    // public List<ChampionshipTeam> QualifiedTeams { get; set; } = [];
}
public enum StageType
{
    Group,
    Final,
}