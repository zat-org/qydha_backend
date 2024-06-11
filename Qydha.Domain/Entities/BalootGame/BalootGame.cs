namespace Qydha.Domain.Entities;

public class BalootGame
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string EventsJsonString { get; set; } = "[]";
    public BalootGameMode GameMode { get; set; }
    public BalootGameData GameData { get; set; } = new BalootGameData();
    public Guid ModeratorId { get; set; }
    public virtual User Moderator { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;

    public List<BalootGameEvent> GetEvents() =>
            JsonConvert.DeserializeObject<List<BalootGameEvent>>(EventsJsonString, BalootConstants.balootEventsSerializationSettings) ?? [];
}


public class InvalidBalootGameActionError(string msg)
    : ResultError(msg, ErrorType.InvalidBalootGameAction, StatusCodes.Status400BadRequest)
{ }