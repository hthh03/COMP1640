using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;

[Authorize]
public class AssignmentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<Users> _userManager;

    public AssignmentsController(AppDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Danh sách bài tập trong khóa học
    public async Task<IActionResult> Index(int courseId)
    {
        var assignments = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();
        return View(assignments);
    }

    // Tạo bài tập (chỉ dành cho giáo viên)
    [Authorize(Roles = "Teacher")]
    public IActionResult Create(int courseId)
    {
        // Truyền courseId vào ViewBag để hiển thị
        ViewBag.CourseId = courseId;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(Assignment assignment)
    {
        // Kiểm tra nếu CourseId hợp lệ
        //if (assignment.CourseId == 0)
        //{
            ModelState.AddModelError("", "Invalid CourseId");
            return View(assignment);
        //}

        // Chuyển DueDate sang UTC
        assignment.DueDate = DateTime.SpecifyKind(assignment.DueDate, DateTimeKind.Utc);

        // Thêm bài tập vào cơ sở dữ liệu
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Chuyển hướng về danh sách bài tập của khóa học
        return RedirectToAction("Index", new { courseId = assignment.CourseId });
    }
}
