using Microsoft.AspNetCore.SignalR;

namespace WebApplication2.Hubs 
{
    public class MeetingHub : Hub
    {
        public async Task JoinRoom(string roomId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserConnected", userId);
        }

        public async Task SendOffer(string roomId, string offer)
        {
            await Clients.Group(roomId).SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string roomId, string answer)
        {
            await Clients.Group(roomId).SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendIceCandidate(string roomId, string candidate)
        {
            await Clients.Group(roomId).SendAsync("ReceiveIceCandidate", candidate);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}