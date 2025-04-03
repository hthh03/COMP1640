using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Posts.Include(p => p.Blog);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.Include(p => p.Blog).FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null) return NotFound();

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Description");
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Content,BlogId")] Post post)
        {           
                post.CreatedAt = DateTime.UtcNow;
                post.UpdatedAt = DateTime.UtcNow;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Description", post.BlogId);
            return View(post);
        }

        //// POST: Posts/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,BlogId")] Post post)
        //{
        //    if (id != post.PostId) return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var existingPost = await _context.Posts.FindAsync(id);
        //            if (existingPost == null) return NotFound();

        //            existingPost.Title = post.Title;
        //            existingPost.Content = post.Content;
        //            existingPost.BlogId = post.BlogId;
        //            existingPost.UpdatedAt = DateTime.UtcNow;

        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PostExists(post.PostId)) return NotFound();
        //            else throw;
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Description", post.BlogId);
        //    return View(post);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,BlogId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            try
            {
                existingPost.Title = post.Title;
                existingPost.Content = post.Content;
                existingPost.BlogId = post.BlogId;
                existingPost.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian sửa đổi

                _context.Update(existingPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.PostId))
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

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.Include(p => p.Blog).FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
