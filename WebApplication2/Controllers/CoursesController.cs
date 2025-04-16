using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Service;
using WebApplication2.Utilities;

[Authorize]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<Users> _userManager;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<CoursesController> _logger;


    public CoursesController(AppDbContext context, UserManager<Users> userManager, IHubContext<NotificationHub> hubContext, IEmailService emailService, ILogger<CoursesController> logger)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
        _emailService = emailService;
        _logger = logger;
    }

    /* public async Task<IActionResult> Index()
     {
         var courses = await _context.Courses.Include(c => c.Teacher).ToListAsync();
         return View(courses);
     } */


    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Teacher)
            .Where(c => c.IsApproved) // chỉ hiển thị khóa học đã được duyệt
            .ToListAsync();

        return View(courses);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        course.IsApproved = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(PendingApprovals));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PendingApprovals()
    {
        var pendingCourses = await _context.Courses
            .Include(c => c.Teacher)
            .Where(c => !c.IsApproved)
            .ToListAsync();

        return View(pendingCourses);
    }


    [Authorize(Roles = "Teacher")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(Course course)
    {
        var teacher = await _userManager.GetUserAsync(User);
        if (teacher == null) return Unauthorized();

        course.TeacherId = teacher.Id;
        course.IsApproved = false; // Khóa học chưa được phê duyệt
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    //[Authorize(Roles = "Student")]
    //public async Task<IActionResult> Join(int id)
    //{
    //    var student = await _userManager.GetUserAsync(User);
    //    if (student == null) return Unauthorized();

    //    var course = await _context.Courses
    //        .Include(c => c.Teacher)
    //        .FirstOrDefaultAsync(c => c.Id == id);

    //    if (course == null) return NotFound();

    //    // Kiểm tra xem sinh viên đã tham gia khóa học chưa
    //    var existingEnrollment = await _context.StudentCourses
    //        .FirstOrDefaultAsync(sc => sc.StudentId == student.Id && sc.CourseId == id);

    //    if (existingEnrollment == null)
    //    {
    //        var studentCourse = new StudentCourses
    //        {
    //            StudentId = student.Id,
    //            CourseId = course.Id
    //        };

    //        _context.StudentCourses.Add(studentCourse);
    //        await _context.SaveChangesAsync();

    //        // Lấy thông tin giáo viên từ AspNetUsers để gửi email
    //        var teacher = await _userManager.FindByIdAsync(course.TeacherId);
    //        if (teacher != null && !string.IsNullOrEmpty(teacher.Email))
    //        {
    //            // Tạo nội dung email
    //            string subject = $"Thông báo: Học viên mới tham gia khóa học {course.Name}";
    //            string body = $@"
    //            <h2>Thông báo từ hệ thống quản lý khóa học</h2>
    //            <p>Xin chào {teacher.UserName},</p>
    //            <p>Học viên <strong>{student.UserName}</strong> (Email: {student.Email}) vừa đăng ký tham gia khóa học <strong>{course.Name}</strong> của bạn.</p>
    //            <p>Thời gian đăng ký: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</p>
    //            <p>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</p>
    //            <p>Trân trọng,<br>Hệ thống quản lý khóa học</p>
    //        ";

    //            // Gửi email thông báo cho giáo viên
    //            bool emailSent = await _emailService.SendEmailAsync(teacher.Email, subject, body);

    //            if (emailSent)
    //            {
    //                _logger.LogInformation($"Notification email sent to teacher {teacher.UserName} for course {course.Name}");
    //            }
    //            else
    //            {
    //                _logger.LogWarning($"Failed to send notification email to teacher {teacher.UserName}");
    //            }

    //            // Thêm thông báo SignalR (giữ nguyên chức năng thông báo realtime)
    //            var message = $"Học viên {student.UserName} vừa tham gia khóa học {course.Name} của bạn!";
    //            await _hubContext.Clients.User(course.TeacherId).SendAsync("ReceiveNotification", message);
    //        }

    //        TempData["Message"] = "You have successfully joined this course.";
    //    }
    //    else
    //    {
    //        TempData["Message"] = "You have already joined this course.";
    //    }

    //    return RedirectToAction(nameof(Index));
    //}

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Join(int id)
    {
        var student = await _userManager.GetUserAsync(User);
        if (student == null) return Unauthorized();

        var course = await _context.Courses
            .Include(c => c.Teacher)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        // Kiểm tra xem sinh viên đã tham gia khóa học hay đã gửi yêu cầu chưa
        var existingEnrollment = await _context.StudentCourses
            .FirstOrDefaultAsync(sc => sc.StudentId == student.Id && sc.CourseId == id);

        var existingRequest = await _context.CourseRequests
            .FirstOrDefaultAsync(cr => cr.StudentId == student.Id && cr.CourseId == id);

        if (existingEnrollment != null)
        {
            TempData["Message"] = "Bạn đã tham gia khóa học này rồi.";
            return RedirectToAction(nameof(Index));
        }

        if (existingRequest != null)
        {
            TempData["Message"] = "Bạn đã gửi yêu cầu tham gia khóa học này. Vui lòng chờ giáo viên phê duyệt.";
            return RedirectToAction(nameof(Index));
        }

        // Tạo yêu cầu tham gia khóa học mới
        var courseRequest = new CourseRequests
        {
            StudentId = student.Id,
            TeacherId = course.TeacherId,
            CourseId = course.Id,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.CourseRequests.Add(courseRequest);
        await _context.SaveChangesAsync();

        // Gửi thông báo cho giáo viên
        var teacher = await _userManager.FindByIdAsync(course.TeacherId);
        if (teacher != null && !string.IsNullOrEmpty(teacher.Email))
        {
            // Tạo nội dung email
            string subject = $"Thông báo: Học viên yêu cầu tham gia khóa học {course.Name}";
            string body = $@"
        <h2>Thông báo từ hệ thống quản lý khóa học</h2>
        <p>Xin chào {teacher.UserName},</p>
        <p>Học viên <strong>{student.UserName}</strong> (Email: {student.Email}) vừa gửi yêu cầu tham gia khóa học <strong>{course.Name}</strong> của bạn.</p>
        <p>Thời gian yêu cầu: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</p>
        <p>Vui lòng đăng nhập vào hệ thống để xem chi tiết và phê duyệt.</p>
        <p>Trân trọng,<br>Hệ thống quản lý khóa học</p>
        ";

            // Gửi email thông báo cho giáo viên
            bool emailSent = await _emailService.SendEmailAsync(teacher.Email, subject, body);

            if (emailSent)
            {
                _logger.LogInformation($"Notification email sent to teacher {teacher.UserName} for course request {course.Name}");
            }
            else
            {
                _logger.LogWarning($"Failed to send notification email to teacher {teacher.UserName}");
            }

            // Thêm thông báo SignalR
            var message = $"Học viên {student.UserName} vừa gửi yêu cầu tham gia khóa học {course.Name} của bạn!";
            await _hubContext.Clients.User(course.TeacherId).SendAsync("ReceiveNotification", message);
        }

        TempData["Message"] = "Yêu cầu tham gia khóa học đã được gửi. Vui lòng chờ giáo viên phê duyệt.";
        return RedirectToAction(nameof(Index));
    }




    // Giáo viên duyệt
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> PendingRequests()
    {
        var teacher = await _userManager.GetUserAsync(User);
        if (teacher == null) return Unauthorized();

        var pendingRequests = await _context.CourseRequests
            .Include(cr => cr.Student)
            .Include(cr => cr.Course)
            .Where(cr => cr.TeacherId == teacher.Id && cr.Status == "Pending")
            .ToListAsync();

        return View(pendingRequests);
    }

    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var request = await _context.CourseRequests
            .Include(cr => cr.Student)
            .Include(cr => cr.Course)
            .FirstOrDefaultAsync(cr => cr.Id == id);

        if (request == null) return NotFound();

        // Kiểm tra xem giáo viên có quyền duyệt không
        var teacher = await _userManager.GetUserAsync(User);
        if (teacher.Id != request.TeacherId) return Forbid();

        // Cập nhật trạng thái yêu cầu
        request.Status = "Approved";

        // Thêm học viên vào khóa học
        var studentCourse = new StudentCourses
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId
        };

        _context.StudentCourses.Add(studentCourse);
        await _context.SaveChangesAsync();

        // Gửi thông báo cho học viên
        var student = await _userManager.FindByIdAsync(request.StudentId);
        if (student != null && !string.IsNullOrEmpty(student.Email))
        {
            string subject = $"Thông báo: Yêu cầu tham gia khóa học {request.Course.Name} đã được chấp nhận";
            string body = $@"
        <h2>Thông báo từ hệ thống quản lý khóa học</h2>
        <p>Xin chào {student.UserName},</p>
        <p>Yêu cầu tham gia khóa học <strong>{request.Course.Name}</strong> của bạn đã được chấp nhận.</p>
        <p>Bạn có thể bắt đầu học ngay bây giờ.</p>
        <p>Trân trọng,<br>Hệ thống quản lý khóa học</p>
        ";

            await _emailService.SendEmailAsync(student.Email, subject, body);

            // Gửi thông báo realtime
            var message = $"Yêu cầu tham gia khóa học {request.Course.Name} của bạn đã được chấp nhận!";
            await _hubContext.Clients.User(request.StudentId).SendAsync("ReceiveNotification", message);
        }

        return RedirectToAction(nameof(PendingRequests));
    }

    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> RejectRequest(int id)
    {
        var request = await _context.CourseRequests
            .Include(cr => cr.Student)
            .Include(cr => cr.Course)
            .FirstOrDefaultAsync(cr => cr.Id == id);

        if (request == null) return NotFound();

        // Kiểm tra xem giáo viên có quyền từ chối không
        var teacher = await _userManager.GetUserAsync(User);
        if (teacher.Id != request.TeacherId) return Forbid();

        // Cập nhật trạng thái yêu cầu
        request.Status = "Rejected";
        await _context.SaveChangesAsync();

        // Gửi thông báo cho học viên
        var student = await _userManager.FindByIdAsync(request.StudentId);
        if (student != null && !string.IsNullOrEmpty(student.Email))
        {
            string subject = $"Thông báo: Yêu cầu tham gia khóa học {request.Course.Name} đã bị từ chối";
            string body = $@"
        <h2>Thông báo từ hệ thống quản lý khóa học</h2>
        <p>Xin chào {student.UserName},</p>
        <p>Yêu cầu tham gia khóa học <strong>{request.Course.Name}</strong> của bạn đã bị từ chối.</p>
        <p>Vui lòng liên hệ với giáo viên để biết thêm chi tiết.</p>
        <p>Trân trọng,<br>Hệ thống quản lý khóa học</p>
        ";

            await _emailService.SendEmailAsync(student.Email, subject, body);

            // Gửi thông báo realtime
            var message = $"Yêu cầu tham gia khóa học {request.Course.Name} của bạn đã bị từ chối.";
            await _hubContext.Clients.User(request.StudentId).SendAsync("ReceiveNotification", message);
        }

        return RedirectToAction(nameof(PendingRequests));
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.StudentCourses)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        return View(course);
    }

    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        return View(course);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course updatedCourse)
    {
        if (id != updatedCourse.Id) return BadRequest();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        course.Name = updatedCourse.Name;
        course.Description = updatedCourse.Description;

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "An error occurred when update the course.");
        }
    }

    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses
            .Include(c => c.StudentCourses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        _context.StudentCourses.RemoveRange(course.StudentCourses);
        _context.Courses.Remove(course);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult SubmitCourseRequest(string studentId, int courseId)
    {
        // Fetch the teacher for this course from the database
        var teacherId = _context.Courses
                           .Where(c => c.Id == courseId)
                           .Select(c => c.TeacherId)
                           .FirstOrDefault();

        // Create a new course request
        var courseRequest = new CourseRequests
        {
            StudentId = studentId,
            TeacherId = teacherId,
            CourseId = courseId,
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        // Save to database
        _context.CourseRequests.Add(courseRequest);
        _context.SaveChanges();

        var message = "A student has requested to join your course!";
        _hubContext.Clients.User(teacherId).SendAsync("ReceiveNotification", message);

        return Ok("Request submitted successfully!");
    }

    [HttpPost]
    public IActionResult UpdateRequestStatus(int requestId, string status)
    {
        var request = _context.CourseRequests.FirstOrDefault(r => r.Id == requestId);
        if (request != null)
        {
            request.Status = status; // "Approved" or "Denied"
            _context.SaveChanges();

            return Ok("Request updated successfully!");
        }

        return NotFound("Request not found!");
    }
}
