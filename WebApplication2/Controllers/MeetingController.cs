using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApplication2.Data;

public class MeetingController : Controller
{
    private readonly AppDbContext _context;

    public MeetingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Create()
    {
        // Lấy danh sách các cuộc họp đã tạo để hiển thị lịch sử
        var meetings = await _context.Meetings
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
        ViewBag.Meetings = meetings;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string teacherEmail, string topic, DateTime startTime)
    {
        // Tạo room name duy nhất
        string roomName = "eTutorMeetingRoom_" + Guid.NewGuid().ToString();
        string joinUrl = Url.Action("Join", "Meeting", new { roomName }, Request.Scheme);

        // Lưu thông tin cuộc họp vào PostgreSQL
        var meeting = new Meeting
        {
            RoomName = roomName,
            TeacherEmail = teacherEmail,
            Topic = topic,
            StartTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc), // Cách 1
                                                                           // hoặc: StartTime = startTime.ToUniversalTime(), // Cách 2 nếu bạn biết startTime là local time
            JoinUrl = joinUrl
        };
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        return RedirectToAction("Join", new { roomName });
    }

    public async Task<IActionResult> Join(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            return RedirectToAction("Create");
        }

        // Lấy danh sách các cuộc họp để hiển thị lịch sử
        var meetings = await _context.Meetings
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
        ViewBag.Meetings = meetings;
        ViewBag.RoomName = roomName;
        return View();
    }
}