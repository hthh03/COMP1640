using Microsoft.AspNetCore.SignalR;

namespace WebApplication2.Hubs
{
    public class MessageHub : Hub
    {
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        // Phương thức để gửi tin nhắn đến người dùng cụ thể
        public async Task SendMessageToUser(string userId, string message, string senderName, string senderId)
        {
            // Gửi tin nhắn đến người nhận
            await Clients.User(userId).SendAsync("ReceiveMessage", message, senderName, senderId);

            // Gửi xác nhận đến người gửi
            await Clients.Caller.SendAsync("MessageSent", userId, message);
        }

        // Khi kết nối mới được thiết lập
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // Khi một kết nối bị ngắt
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
