using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Hubs.BroadcastHub
{
    public class BroadcastHub : Hub
    {
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("employee-logged");
        }
    }
}