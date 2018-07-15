using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Hubs.BroadcastHub
{
    public class BroadcastHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("send", message);
        }
    }
}