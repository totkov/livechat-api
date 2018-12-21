using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace LiveChat.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            ConnectionHelper.AddConnection(Context.ConnectionId, this.LoggedUserId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionHelper.RemoveConnection(this.LoggedUserId);

            return base.OnDisconnectedAsync(exception);
        }

        private int LoggedUserId
        {
            get
            {
                try
                {
                    var userIdClaim = Context.User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
                    if (userIdClaim != null)
                    {
                        return int.Parse(userIdClaim.Value);
                    }

                    return -1;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }
    }
}
