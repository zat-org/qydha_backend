using Microsoft.AspNetCore.SignalR;
using Qydha.Domain.Mappers;

namespace Qydha.Domain.MediatorHandlers;

public class UserDataChangedHandler(IHubContext<UsersHub, IUserClient> hubContext) : INotificationHandler<UserDataChangedNotification>
{
    private readonly IHubContext<UsersHub, IUserClient> _hubContext = hubContext;
    public async Task Handle(UserDataChangedNotification notification, CancellationToken cancellationToken)
    {
        var mapper = new UserStreamMapper();
        string serializedUser = JsonConvert.SerializeObject(mapper.UserToUserDto(notification.User), BalootConstants.balootEventsSerializationSettings);
        await _hubContext.Clients.User(notification.User.Id.ToString()).UserDataChanged(serializedUser);
    }
}
