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
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public CommentController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Xử lý thêm bình luận mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var comment = new Comment
                {
                    Content = model.Content,
                    CreatedAt = DateTime.Now,
                    PostId = model.PostId,
                    UserId = userId
                };

                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Post", new { id = model.PostId });
            }

            return RedirectToAction("Details", "Post", new { id = model.PostId });
        }

        // Hiển thị form chỉnh sửa bình luận
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId)
            {
                return Forbid();
            }

            var commentViewModel = new CommentViewModel
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                PostId = comment.PostId
            };

            return View(commentViewModel);
        }

        // Xử lý chỉnh sửa bình luận
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, CommentViewModel model)
        {
            if (id != model.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var comment = await _context.Comments.FindAsync(id);
                    if (comment == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (comment.UserId != userId)
                    {
                        return Forbid();
                    }

                    comment.Content = model.Content;
                    comment.UpdatedAt = DateTime.Now;

                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(model.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Post", new { id = model.PostId });
            }
            return View(model);
        }

        // Xử lý xóa bình luận
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId)
            {
                return Forbid();
            }

            var postId = comment.PostId;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Post", new { id = postId });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}

