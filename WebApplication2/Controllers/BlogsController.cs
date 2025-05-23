﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BlogsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public BlogsController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class BlogModelBind
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Blogs.Include(b => b.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null) return NotFound();

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CreatedAt")] BlogModelBind blog)
        {
            if (ModelState.IsValid)
            {
                var currentuser = await _userManager.GetUserAsync(User);

                var newBlog = new Blog
                {
                    Title = blog.Title,
                    Description = blog.Description,
                    CreatedAt = DateTime.UtcNow,
                    UserId = currentuser.Id
                };

                _context.Add(newBlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blog.UserId);
            return View(blog);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Title,Description,CreatedAt,UserId")] Blog blog)
        {
            if (id != blog.BlogId) return NotFound();

            var existingBlog = await _context.Blogs.FindAsync(id);
            if (existingBlog == null) return NotFound();

            try
            {
                existingBlog.Title = blog.Title;
                existingBlog.Description = blog.Description;
                existingBlog.UserId = blog.UserId;

                // 👇 Đây là dòng quan trọng – ép kiểu UTC
                existingBlog.CreatedAt = DateTime.SpecifyKind(blog.CreatedAt, DateTimeKind.Utc);

                _context.Update(existingBlog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(blog.BlogId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null) return NotFound();

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}
