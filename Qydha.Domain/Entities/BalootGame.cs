namespace Qydha.Domain.Entities;

public class BalootGame
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string EventsJsonString { get; set; } = "[]";

    public BalootGameMode GameMode { get; set; }

    #region relations
    public Guid ModeratorId { get; set; }
    public virtual User Moderator { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;

    #endregion
    public List<BalootGameEvent> GetEvents() =>
            JsonConvert.DeserializeObject<List<BalootGameEvent>>(EventsJsonString, BalootConstants.balootEventsSerializationSettings) ?? [];

}

public enum BalootGameMode
{
    SinglePlayer,
    MultiPlayer,
    Championship,
}