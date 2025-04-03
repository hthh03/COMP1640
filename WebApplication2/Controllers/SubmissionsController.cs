using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;
using Microsoft.AspNetCore.Http;

[Authorize(Roles = "Student")]
public class SubmissionController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<Users> _userManager;

    public SubmissionController(AppDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Trang nộp bài
    public IActionResult Submit(int assignmentId)
    {
        ViewBag.AssignmentId = assignmentId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Submit(int assignmentId, IFormFile file)
    {
        var student = await _userManager.GetUserAsync(User);
        if (student == null) return Unauthorized();

        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Please upload a file.");
            return View();
        }

        // Lưu file vào thư mục
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        var filePath = Path.Combine(uploads, file.FileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        // Lưu thông tin bài nộp vào CSDL
        var submission = new Submission
        {
            AssignmentId = assignmentId,
            StudentId = student.Id,
            FilePath = "/uploads/" + file.FileName,
            SubmittedAt = DateTime.UtcNow
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Assignment", new { courseId = assignmentId });
    }
}
