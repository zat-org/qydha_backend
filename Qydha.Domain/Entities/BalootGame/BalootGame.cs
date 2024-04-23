namespace Qydha.Domain.Entities;

public class BalootGame
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string EventsJsonString { get; set; } = "[]";
    public BalootGameMode GameMode { get; set; }
    public BalootGameState State { get; set; } = new BalootGameState();
    public Guid ModeratorId { get; set; }
    public virtual User Moderator { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;

    public List<BalootGameEvent> GetEvents() =>
            JsonConvert.DeserializeObject<List<BalootGameEvent>>(EventsJsonString, BalootConstants.balootEventsSerializationSettings) ?? [];

}