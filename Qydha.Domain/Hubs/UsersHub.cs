using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Algorithm;

namespace Qydha.Domain.Hubs;

public interface IUserClient
{
    Task UserDataChanged(string userData);
}
[Authorize(Roles = nameof(UserRoles.User))]
public class UsersHub() : Hub<IUserClient>
{

}