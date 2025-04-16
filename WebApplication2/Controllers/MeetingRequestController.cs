using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;


namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Student")]
    public class MeetingRequestController : Controller
    {
        private readonly AppDbContext _context; // Replace with your DbContext
        private readonly IHubContext<Hub> _hubContext; // Replace with your SignalR hub

        public MeetingRequestController(AppDbContext context, IHubContext<Hub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> RequestMeeting(string teacherEmail, string topic, DateTime preferredTime)
        {
            var request = new MeetingRequest
            {
                StudentEmail = User.Identity.Name,
                TeacherEmail = teacherEmail,
                Topic = topic,
                PreferredTime = preferredTime,
                Status = "Pending"
            };

            _context.MeetingRequests.Add(request);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(teacherEmail)
                .SendAsync("ReceiveMeetingRequest", new
                {
                    Student = request.StudentEmail,
                    Topic = topic,
                    Time = preferredTime.ToString("g")
                });

            return RedirectToAction("StudentDashboard");
        }
    }
}