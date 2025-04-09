using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApplication2.Data;


[Authorize]
public class MeetingController : Controller
{
    private readonly AppDbContext _context;

    public MeetingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Create()
    {

        var meetings = await _context.Meetings
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
        ViewBag.Meetings = meetings;
        return View();
    }

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

        return RedirectToAction("Join", new { roomName });
    }
    
    public async Task<IActionResult> Join(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            return RedirectToAction("Create");
        }


        var meetings = await _context.Meetings
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
        ViewBag.Meetings = meetings;
        ViewBag.RoomName = roomName;
        return View();
    }
}