using WebApplication2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using WebApplication2.Service;

namespace WebApplication2.Controllers
{
    // [Authorize(Roles = "Admin")] // Chỉ Admin mới truy cập được
    public class AdminController : Controller
    {
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailService _emailService;

        public AdminController(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _emailService = emailService;
        }

        // Hiển thị danh sách người dùng với tính năng tìm kiếm và lọc
        public async Task<IActionResult> Index(string searchString, string roleFilter)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentRoleFilter"] = roleFilter;

            var users = userManager.Users;

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString) ||
                                        u.Email.Contains(searchString) ||
                                        u.FullName.Contains(searchString));
            }

            // Lọc theo vai trò
            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            // Lấy danh sách tất cả các vai trò
            var roles = new List<string> { "Admin", "Teacher", "Student" };
            ViewData["Roles"] = roles;

            return View(await users.ToListAsync());
        }

        // Hiển thị form tạo người dùng mới
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // Xóa tài khoản người dùng
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "User not found!";
            }
            return RedirectToAction("Index");
        }

        // Thay đổi Role của người dùng
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, currentRoles);
                await userManager.AddToRoleAsync(user, newRole);

                user.Role = newRole;
                await userManager.UpdateAsync(user);

                TempData["Success"] = "User role updated successfully!";
            }
            else
            {
                TempData["Error"] = "User not found!";
            }
            return RedirectToAction("Index");
        }

        // Tạo người dùng mới với Role Student/Teacher
        //[HttpPost]
        //public async Task<IActionResult> CreateUser(string FullName, string Email, string Password, string Role)
        //{
        //    // Kiểm tra dữ liệu đầu vào
        //    if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
        //        string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
        //    {
        //        TempData["Error"] = "All fields are required!";
        //        return RedirectToAction("Index");
        //    }

        //    // Kiểm tra xem Email đã tồn tại chưa
        //    var existingUser = await userManager.FindByEmailAsync(Email);
        //    if (existingUser != null)
        //    {
        //        TempData["Error"] = $"Email {Email} already exists!";
        //        return RedirectToAction("Index");
        //    }

        //    // Kiểm tra Role hợp lệ trước khi tạo
        //    if (!await roleManager.RoleExistsAsync(Role))
        //    {
        //        var newRole = new IdentityRole(Role);
        //        await roleManager.CreateAsync(newRole);
        //    }

        //    var user = new Users
        //    {
        //        UserName = Email,
        //        Email = Email,
        //        FullName = FullName,
        //        Role = Role // Gán Role vào User
        //    };

        //    var result = await userManager.CreateAsync(user, Password);

        //    if (result.Succeeded)
        //    {
        //        await userManager.AddToRoleAsync(user, Role);
        //        await userManager.UpdateAsync(user);

        //        TempData["Success"] = "User created successfully!";
        //    }
        //    else
        //    {
        //        TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
        //    }

        //    return RedirectToAction("Index");
        //}

        // Tạo người dùng mới với Role Student/Teacher

        [HttpPost]
        public async Task<IActionResult> CreateUser(string FullName, string Email, string Password, string Role)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                TempData["Error"] = "All fields are required!";
                return RedirectToAction("Index");
            }

            var existingUser = await userManager.FindByEmailAsync(Email);
            if (existingUser != null)
            {
                TempData["Error"] = $"Email {Email} already exists!";
                return RedirectToAction("Index");
            }

            if (!await roleManager.RoleExistsAsync(Role))
            {
                var newRole = new IdentityRole(Role);
                await roleManager.CreateAsync(newRole);
            }

            var user = new Users
            {
                UserName = Email,
                Email = Email,
                FullName = FullName,
                Role = Role
            };

            var result = await userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Role);
                await userManager.UpdateAsync(user);

                // ✅ Gửi email thông báo
                await _emailService.SendEmailAsync(user.Email, "Tài khoản đã được tạo",
                    $"Chào {FullName},<br/><br/>Tài khoản của bạn đã được tạo thành công với vai trò <b>{Role}</b>.<br/><br/>Thông tin đăng nhập:<br/>Email: {Email}<br/>Mật khẩu: (đã được đặt khi tạo)<br/><br/>Vui lòng đăng nhập để bắt đầu.");

                TempData["Success"] = "User created and email sent successfully!";
            }
            else
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index");
        }
    }
}
