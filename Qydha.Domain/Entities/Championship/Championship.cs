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
    public string FinalStageCity { get; set; } = null!;
    public Point FinalStageLocation { get; set; } = null!;
    public DateOnly FinalStageStartAt { get; set; }
    public DateOnly FinalStageEndAt { get; set; }
    public bool FinalStageWith3rdPlaceMatch { get; set; }
    public int FinalStageNumberOfParticipants { get; set; }
    public GroupStage? GroupStage { get; set; }
    public List<ChampionshipTeam> FinalStageQualifiedTeams { get; set; } = [];
    public List<ChampionshipMatch> FinalStageMatches { get; set; } = [];
    public List<ChampionshipTeam> FinalStageWinners { get; set; } = [];
}
public class ChampionshipTeamJoinRequest
{
    public int Id { get; set; }
    public string FirstPlayerName { get; set; } = null!;
    public string FirstPlayerPhone { get; set; } = null!;
    public DateTimeOffset? FirstPlayerConfirmation { get; set; }
    public string SecondPlayerName { get; set; } = null!;
    public string SecondPlayerPhone { get; set; } = null!;
    public DateTimeOffset? SecondPlayerConfirmation { get; set; }
    public int ChampionshipId { get; set; }
    public Championship Championship { get; set; } = null!;
}

public class GroupStage()
{
    public string City { get; set; } = null!;
    public Point Location { get; set; } = null!;
    public DateOnly StartAt { get; set; }
    public DateOnly EndAt { get; set; }
    public int WinnersCountPerGroup { get; set; }
    public List<ChampionshipTeam> QualifiedTeams { get; set; } = [];
    public List<ChampionshipQualificationGroup> Groups { get; set; } = [];
}
public class ChampionshipTable
{
    public int Id { get; set; }
    public Guid RefereeId { get; set; }
    // public User Referee { get; set; } = null!;
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
    public int TableId { get; set; }
    public ChampionshipTable Table { get; set; } = null!;

}
public class ChampionshipQualificationGroup
{
    public int Id { get; set; }
    public List<ChampionshipMatch> Matches { get; set; } = [];
    public List<ChampionshipTeam> Winners { get; set; } = [];
    public List<ChampionshipTeam> QualifiedTeams { get; set; } = [];
}
