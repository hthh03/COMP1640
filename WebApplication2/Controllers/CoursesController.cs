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

    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses.Include(c => c.Teacher).ToListAsync();
        return View(courses);
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
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Join(int id)
    {
        var student = await _userManager.GetUserAsync(User);
        if (student == null) return Unauthorized();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        // Kiểm tra xem sinh viên đã tham gia khóa học chưa
        var existingEnrollment = await _context.StudentCourses
            .FirstOrDefaultAsync(sc => sc.StudentId == student.Id && sc.CourseId == id);

        if (existingEnrollment == null)
        {
            var studentCourse = new StudentCourse
            {
                StudentId = student.Id,
                CourseId = course.Id
            };

            _context.StudentCourses.Add(studentCourse);
            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["Message"] = "You have already joined this course.";
        }

        return RedirectToAction(nameof(Index));
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
            return StatusCode(500, "Lỗi khi cập nhật khóa học.");
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
}
