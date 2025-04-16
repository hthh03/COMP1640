using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace WebApplication2.Hubs
{
    public class MeetingHub : Hub
    {
        // Khi người dùng kết nối, phân nhóm theo vai trò (Teacher/Student)
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null)
            {
                if (user.IsInRole("Teacher"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Teachers");
                }
                else if (user.IsInRole("Student"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Students");
                }
            }

            await base.OnConnectedAsync();
        }

        // Khi người dùng rời khỏi
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // Dùng cho video call WebRTC
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

        // Thông báo cuộc họp mới cho học sinh
        public async Task NotifyNewMeeting(string topic, string teacherEmail, string startTime)
        {
            await Clients.Group("Students").SendAsync("ReceiveMeetingNotification", new
            {
                Topic = topic,
                Teacher = teacherEmail,
                Time = startTime
            });
        }

        // Gửi yêu cầu cuộc họp đến giáo viên
        public async Task SendMeetingRequest(string teacherEmail, string studentEmail, string topic, string time)
        {
            await Clients.User(teacherEmail).SendAsync("ReceiveMeetingRequest", new
            {
                Student = studentEmail,
                Topic = topic,
                Time = time
            });
        }
    }
}
