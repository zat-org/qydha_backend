using Microsoft.AspNetCore.SignalR;

namespace Qydha.Domain.Hubs;

public interface IBalootGameClient
{
    Task BalootGameStateChanged(string eventName, string game);
}
public class BalootGamesHub(IBalootGameStreamService balootGameStreamService) : Hub<IBalootGameClient>
{
    private readonly IBalootGameStreamService _balootGameStreamService = balootGameStreamService;
    public async Task<string> AddToBoardGroup(Guid boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
        var res = await _balootGameStreamService.GetGameByBoardId(boardId);
        if (res.IsSuccess)
        {
            return JsonConvert.SerializeObject(res.Value, BalootConstants.balootEventsSerializationSettings);
        }
        return string.Empty;
    }

}