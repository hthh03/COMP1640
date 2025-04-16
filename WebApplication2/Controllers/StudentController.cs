using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RequestMeeting(string email, string message, DateTime meetingTime)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("Dashboard");
            }

            var request = new MeetingRequest
            {
                StudentEmail = email,
                TeacherEmail = email, // Thêm tham số này vào phương thức của bạn
                Topic = message,
                Message = message, // Thêm thuộc tính này
                MeetingTime = meetingTime.ToUniversalTime(), // Chuyển sang UTC
                RequestDate = DateTime.UtcNow, // Sử dụng UtcNow thay vì Now
                Status = "Pending" // Thêm trạng thái mặc định
            };

            _context.MeetingRequests.Add(request);
            _context.SaveChanges();

            TempData["Success"] = "Yêu cầu họp đã được gửi thành công!";
            return RedirectToAction("Dashboard");
        }
    }
}