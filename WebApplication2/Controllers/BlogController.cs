using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.ViewModels;



namespace WebApplication2.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public BlogController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị tất cả các blog
        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Include(b => b.User)
                .Select(b => new BlogViewModel
                {
                    BlogId = b.BlogId,
                    Title = b.Title,
                    Description = b.Description,
                    UserName = b.User.FullName,
                    UserRole = b.User.Role
                })
                .ToListAsync();

            return View(blogs);
        }

        // Xem chi tiết blog và các bài post thuộc blog
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Posts)
                .FirstOrDefaultAsync(b => b.BlogId == id);

            if (blog == null)
            {
                return NotFound();
            }

            var blogViewModel = new BlogViewModel
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
                UserName = blog.User.FullName,
                UserRole = blog.User.Role
            };

            var posts = await _context.Posts
                .Where(p => p.BlogId == id)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostViewModel
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            ViewBag.Posts = posts;
            ViewBag.BlogId = id;

            return View(blogViewModel);
        }

        // Hiển thị form tạo blog mới
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo blog mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(BlogViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                var blog = new Blog
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedAt = DateTime.Now,
                    UserId = userId
                };

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Hiển thị form chỉnh sửa blog
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blog.UserId != userId)
            {
                return Forbid();
            }

            var blogViewModel = new BlogViewModel
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description
            };

            return View(blogViewModel);
        }

        // Xử lý chỉnh sửa blog
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, BlogViewModel model)
        {
            if (id != model.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var blog = await _context.Blogs.FindAsync(id);
                    if (blog == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (blog.UserId != userId)
                    {
                        return Forbid();
                    }

                    blog.Title = model.Title;
                    blog.Description = model.Description;

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(model.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Hiển thị trang xác nhận xóa blog
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blog.UserId != userId)
            {
                return Forbid();
            }

            return View(blog);
        }

        // Xử lý xóa blog
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blog.UserId != userId)
            {
                return Forbid();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}
