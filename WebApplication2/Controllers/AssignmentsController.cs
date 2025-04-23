using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;


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

        ViewBag.CourseId = courseId;
        return View(assignments);
    }

    // Tạo bài tập (GET)
    [Authorize(Roles = "Teacher")]
    public IActionResult Create(int courseId)
    {
        var assignment = new Assignment
        {
            CourseId = courseId
        };

        return View(assignment);
    }

    // Tạo bài tập (POST)
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(Assignment assignment)
    {
        // Kiểm tra nếu CourseId tồn tại
        if (!_context.Courses.Any(c => c.Id == assignment.CourseId))
        {
            ModelState.AddModelError("", "Invalid CourseId");
            return View(assignment);
        }

        // Chuyển DueDate sang UTC
        assignment.DueDate = DateTime.SpecifyKind(assignment.DueDate, DateTimeKind.Utc);

        // Thêm bài tập vào cơ sở dữ liệu
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Chuyển hướng về danh sách bài tập
        return RedirectToAction("Index", new { courseId = assignment.CourseId });
    }
}
