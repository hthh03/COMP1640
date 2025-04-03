using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;

[Authorize]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<Users> _userManager;

    public CoursesController(AppDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Danh sách tất cả khóa học
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses.Include(c => c.Teacher).ToListAsync();
        return View(courses);
    }

    // Tạo khóa học (chỉ dành cho giáo viên)
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
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Học sinh tham gia khóa học
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Join(int id)
    {
        var student = await _userManager.GetUserAsync(User);
        if (student == null) return Unauthorized();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        var studentCourse = new StudentCourse
        {
            StudentId = student.Id,
            CourseId = course.Id
        };

        _context.StudentCourses.Add(studentCourse);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.StudentCourses)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

}
