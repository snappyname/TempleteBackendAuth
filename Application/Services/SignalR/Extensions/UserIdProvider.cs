using Microsoft.AspNetCore.SignalR;
using TemplateWebApi;

namespace Application.Services.SignalR.Extensions
{
    public class UserIdProvider: IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(UserClaimTypes.UserId)?.Value;
        }
    }
}
