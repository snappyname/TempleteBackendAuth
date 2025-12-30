using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services.SignalR.Extensions
{
    [Authorize]
    public class BroadcastHub : Hub
    {
      
    }
}
