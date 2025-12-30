namespace Application.Abstract.SignalR
{
    public interface IBroadcastService
    {
        Task SendToUserAsync<T>(string userId, string messageType, T payload);
        Task SendToUsersAsync<T>(List<string> userIds, string messageType, T payload);
    }
}
