using Microsoft.AspNetCore.SignalR;
using Qydha.Domain.Mappers;

namespace Qydha.Domain.MediatorHandlers;

public class ApplyEventsToBalootGameHandler(IHubContext<BalootGamesHub, IBalootGameClient> hubContext, IBalootGameStreamService balootGameStreamService)
    : INotificationHandler<ApplyEventsToBalootGameNotification>
{
    private readonly IBalootGameStreamService _balootGameStreamService = balootGameStreamService;
    private readonly IHubContext<BalootGamesHub, IBalootGameClient> _hubContext = hubContext;
    public async Task Handle(ApplyEventsToBalootGameNotification notification, CancellationToken cancellationToken)
    {
        (await _balootGameStreamService.CreateBalootGameStreamEvent(notification.UserId, notification.Game))
        .OnSuccessAsync(async (e) =>
        {
            string serializedGame = JsonConvert.SerializeObject(e.Game, BalootConstants.balootEventsSerializationSettings);
            await _hubContext.Clients.Group(e.BoardId.ToString()).BalootGameStateChanged(e.EventName, serializedGame);
        });
    }
}