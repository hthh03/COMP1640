using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Hubs;
using WebApplication2.Models; // Giả định bạn có Meeting model ở đây

[Authorize]
public class MeetingController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHubContext<MeetingHub> _hubContext;

    public MeetingController(AppDbContext context, IHubContext<MeetingHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // Hiển thị danh sách cuộc họp
    public async Task<IActionResult> Create()
    {
        var meetings = await _context.Meetings
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();

        ViewBag.Meetings = meetings;
        return View();
    }

    // POST: Tạo cuộc họp mới và thông báo cho học sinh
    [HttpPost]
    public async Task<IActionResult> Create(string teacherEmail, string topic, DateTime startTime)
    {
        string roomName = "eTutorMeetingRoom_" + Guid.NewGuid().ToString();
        string joinUrl = Url.Action("Join", "Meeting", new { roomName }, Request.Scheme);

        var meeting = new Meeting
        {
            RoomName = roomName,
            TeacherEmail = teacherEmail,
            Topic = topic,
            StartTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc),
            JoinUrl = joinUrl
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Gửi thông báo SignalR cho nhóm Students
        await _hubContext.Clients.Group("Students").SendAsync("ReceiveMeetingNotification", new
        {
            Topic = topic,
            Teacher = teacherEmail,
            Time = startTime.ToString("yyyy-MM-dd HH:mm")
        });

        return RedirectToAction("Join", new { roomName });
    }

    // Tham gia phòng họp
    public async Task<IActionResult> Join(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            return RedirectToAction("Create");
        }

        var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.RoomName == roomName);
        if (meeting == null)
        {
            return NotFound("Meeting not found.");
        }

        ViewBag.RoomName = roomName;
        ViewBag.Meeting = meeting;

        return View();
    }
}
