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
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public PostController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị chi tiết bài post
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Blog.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            var postViewModel = new PostViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                BlogId = post.BlogId,
                BlogTitle = post.Blog.Title,
                AuthorName = post.Blog.User.FullName,
                Comments = post.Comments.Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User.FullName,
                    PostId = c.PostId
                }).ToList()
            };

            return View(postViewModel);
        }

        // Hiển thị form tạo bài post mới
        [Authorize]
        public async Task<IActionResult> Create(int blogId)
        {
            var blog = await _context.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blog.UserId != userId)
            {
                return Forbid();
            }

            ViewBag.BlogId = blogId;
            return View();
        }

        // Xử lý tạo bài post mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PostViewModel model, int blogId)
        {
            if (ModelState.IsValid)
            {
                var blog = await _context.Blogs.FindAsync(blogId);
                if (blog == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (blog.UserId != userId)
                {
                    return Forbid();
                }

                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    CreatedAt = DateTime.Now,
                    BlogId = blogId
                };

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Blog", new { id = blogId });
            }

            ViewBag.BlogId = blogId;
            return View(model);
        }

        // Hiển thị form chỉnh sửa bài post
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.Blog.UserId != userId)
            {
                return Forbid();
            }

            var postViewModel = new PostViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                BlogId = post.BlogId
            };

            return View(postViewModel);
        }

        // Xử lý chỉnh sửa bài post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PostViewModel model)
        {
            if (id != model.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var post = await _context.Posts
                    .Include(p => p.Blog)
                        .FirstOrDefaultAsync(p => p.PostId == id);

                    if (post == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (post.Blog.UserId != userId)
                    {
                        return Forbid();
                    }

                    post.Title = model.Title;
                    post.Content = model.Content;
                    post.UpdatedAt = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(model.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = model.PostId });
            }
            return View(model);
        }

        // Hiển thị trang xác nhận xóa bài post
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.Blog.UserId != userId)
            {
                return Forbid();
            }

            return View(post);
        }

        // Xử lý xóa bài post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.Blog.UserId != userId)
            {
                return Forbid();
            }

            var blogId = post.BlogId;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Blog", new { id = blogId });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}

