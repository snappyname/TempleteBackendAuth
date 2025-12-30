using Application.Abstract;
using Application.Abstract.SignalR;
using Application.Services.SignalR.Extensions;
using DTO;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services.SignalR
{
    public class BroadcastService : IBroadcastService
    {
        private readonly IHubContext<BroadcastHub> _hubContext;
        private const string BroadcastMethod = "broadcast";

        public BroadcastService(IHubContext<BroadcastHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync<T>(string userId, string messageType, T payload)
        {
            var message = new BroadcastMessageModel<T>() { Type = messageType, Payload = payload };
            await _hubContext.Clients.User(userId).SendAsync(BroadcastMethod, message);
        }

        public async Task SendToUsersAsync<T>(List<string> userIds, string messageType, T payload)
        {
            var message = new BroadcastMessageModel<T>() { Type = messageType, Payload = payload };
            await _hubContext.Clients.Users(userIds).SendAsync(BroadcastMethod, message);
        }
    }
}
