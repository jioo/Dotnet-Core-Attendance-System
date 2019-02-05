using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hubs.BroadcastHub
{
    [Authorize]
    public class BroadcastHub : Hub
    {
    }
}