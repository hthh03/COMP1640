using Microsoft.AspNetCore.SignalR;

namespace WebApplication2.Utilities
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string teacherId, string message)
        {
            await Clients.User(teacherId).SendAsync("ReceiveNotification", message);
        }
    }
}
