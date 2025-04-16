using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Teacher, Admin")]
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // Lấy email của giáo viên hiện tại (giả sử bạn đang sử dụng ASP.NET Identity)
            var teacherEmail = User.Identity.Name; // Điều chỉnh theo cách xác thực của bạn

            // Lấy tất cả yêu cầu họp đang chờ của giáo viên
            var meetingRequests = _context.MeetingRequests
                .Where(m => m.TeacherEmail == teacherEmail && m.Status == "Pending")
                .OrderBy(m => m.RequestDate)
                .ToList();

            return View(meetingRequests);
        }
        public IActionResult RequestMeeting()
        {
            // Lấy email của giáo viên hiện tại (giả sử bạn đang sử dụng ASP.NET Identity)
            var teacherEmail = User.Identity.Name; // Điều chỉnh nếu bạn dùng cách xác thực khác

            // Lấy tất cả yêu cầu họp của giáo viên (bao gồm cả trạng thái Pending, Accepted, Rejected)
            var meetingRequests = _context.MeetingRequests
                .Where(m => m.TeacherEmail == teacherEmail)
                .OrderBy(m => m.RequestDate)
                .ToList();

            return View(meetingRequests);
        }
        // Tùy chọn: Action để xử lý chấp nhận/từ chối yêu cầu họp
        [HttpPost]
        public IActionResult UpdateMeetingRequest(int id, string status)
        {
            var request = _context.MeetingRequests.Find(id);
            if (request == null)
            {
                TempData["Error"] = "Yêu cầu không tồn tại.";
                return RedirectToAction("RequestMeeting");
            }

            request.Status = status; // "Accepted" hoặc "Rejected"
            _context.SaveChanges();

            TempData["Success"] = $"Yêu cầu đã được {status}.";
            return RedirectToAction("RequestMeeting");
        }
    }
}